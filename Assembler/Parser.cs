using Dankle;
using Dankle.Components;
using Dankle.Components.Arguments;
using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Assembler
{
	public class Parser : BaseParser<Token, Token.Type>
	{
		public readonly Dictionary<Type, Dictionary<string, object>> Variables = [];
		public readonly Dictionary<uint, byte[]> Data = [];
		public readonly HashSet<string> ExportedSymbols = [];

		public uint StartAddr;
		public uint Addr { get; private set; }

		private readonly Dictionary<Type, ArgumentParser> ArgParsers = [];

		private bool IsSymbolPass = true;

		public Parser(List<Token> tokens, uint startAddress = 0, Computer? computer = null) : base(tokens)
		{
			StartAddr = startAddress;

			ArgParsers[typeof(Register)] = new RegisterParser(this);
			ArgParsers[typeof(DoubleRegister)] = new DoubleRegParser(this);
			ArgParsers[typeof(Any8Num)] = new Any8NumParser(this);
			ArgParsers[typeof(Any16Num)] = new Any16NumParser(this);
			ArgParsers[typeof(Any16)] = new Any16Parser(this);
			ArgParsers[typeof(Any32)] = new Any32Parser(this);
			ArgParsers[typeof(Any64)] = new Any64Parser(this);
			ArgParsers[typeof(Pointer<byte>)] = new PointerParser(this);
			ArgParsers[typeof(Pointer<ushort>)] = new PointerParser(this);
			ArgParsers[typeof(Pointer<uint>)] = new PointerParser(this);
			ArgParsers[typeof(Pointer<ulong>)] = new PointerParser(this);

			if (computer is not null) SetVariablesForComputer(computer);
		}

		public override void Parse()
		{
			SymbolPass();
			ParsingPass();
		}

		public uint SymbolPass()
		{
			Addr = StartAddr;
			var backup = new Queue<Token>(Tokens);

			IsSymbolPass = true;
			ParsingPass();
			IsSymbolPass = false;

			Tokens = backup;
			Data.Clear();

			var size = Addr - StartAddr;
			Addr = StartAddr;

			return size;
		}

		public void ApplySymbols(Parser other)
		{
			foreach (var i in other.ExportedSymbols)
			{
				SetVariable(i, other.GetVariable<uint>(i));
			}
		}

		private void ParsingPass()
		{
			while (Tokens.Count > 0)
			{
				var token = Tokens.Dequeue();

				if (token.Symbol == Token.Type.Text)
				{
					var insn = Instruction.Get(token.Text);

					var argTypes = new byte[] { 0, 0, 0, 0 };
					List<byte> argData = [];

					for (int idex = 0; idex < insn.Arguments.Length; idex++)
					{
						if (idex != 0)
						{
							var comma = Tokens.Dequeue();
							if (comma.Symbol != Token.Type.Comma) throw new InvalidTokenException(comma, Token.Type.Comma);
						}

						var (type, data) = ArgParsers[insn.Arguments[idex]].Parse();
						argTypes[idex] = type;
						argData.AddRange(data);
					}

					Data[Addr] =
					[
						..Utils.ToBytes(insn.Opcode),
						(byte)((argTypes[0] << 4) | argTypes[1]),
						(byte)((argTypes[2] << 4) | argTypes[3]),
						..argData
					];

					Addr += (uint)(4 + argData.Count);
				}
				else if (token.Symbol == Token.Type.Label)
				{
					SetVariable(token.Text, Addr);
				}
				else if (token.Symbol == Token.Type.String)
				{
					Data[Addr] = [.. Encoding.UTF8.GetBytes(ParseString(token)), 0];
					Addr += (uint)Data[Addr].Length;
				}
				else if (token.Symbol == Token.Type.Integer)
				{
					Data[Addr] = [ParseNum<byte>(token)];
					Addr++;
				}
				else if (token.Symbol == Token.Type.Export)
				{
					ExportedSymbols.Add(GetNextToken(Token.Type.Text).Text);
				}
				else throw new InvalidTokenException(token);
			}
		}

		public string ParseString(Token? token = null)
		{
			token ??= Tokens.Dequeue();
			Assume(token.Value, Token.Type.String);

			var text = token.Value.Text[1..(token.Value.Text.Length - 1)];
			return ProcessString(text);
		}

		public byte ParseRegister(Token? token = null)
		{
			token ??= Tokens.Dequeue();
			Assume(token.Value, Token.Type.Register);
			return byte.Parse(token.Value.Text[1..]);
		}

		public byte ParseDoubleRegister(Token? reg1 = null, Token? comma = null, Token? reg2 = null)
		{
			reg1 ??= Tokens.Dequeue();

			if (reg1.Value.Symbol == Token.Type.Text)
			{
				if (reg1.Value.Text.Equals("SP", StringComparison.CurrentCultureIgnoreCase)) return 0xCD;
				if (reg1.Value.Text.Equals("PC", StringComparison.CurrentCultureIgnoreCase)) return 0xEF;
			}

			comma ??= Tokens.Dequeue();
			reg2 ??= Tokens.Dequeue();

			Assume(reg1.Value, Token.Type.Register);
			Assume(comma.Value, Token.Type.Comma);
			Assume(reg2.Value, Token.Type.Register);

			return (byte)((ParseRegister(reg1) << 4) | ParseRegister(reg2));
		}

		public byte ParseStandaloneDoubleRegister(Token? paren = null)
		{
			paren ??= Tokens.Dequeue();
			Assume(paren.Value, Token.Type.OParen);
			var ret = ParseDoubleRegister();
			GetNextToken(Token.Type.CParen);
			return ret;
		}

		public ushort ParseQuadRegister()
		{
			var reg1 = GetNextToken(Token.Type.Register);
			GetNextToken(Token.Type.Comma);
			var reg2 = GetNextToken(Token.Type.Register);
			GetNextToken(Token.Type.Comma);
			var reg3 = GetNextToken(Token.Type.Register);
			GetNextToken(Token.Type.Comma);
			var reg4 = GetNextToken(Token.Type.Register);

			return (ushort)((ParseRegister(reg1) << 12) | (ParseRegister(reg2) << 8) | (ParseRegister(reg3) << 4) | ParseRegister(reg4));
		}

		public ushort ParseStandaloneQuadRegister(Token? paren = null)
		{
			paren ??= Tokens.Dequeue();
			Assume(paren.Value, Token.Type.OParen);
			var ret = ParseQuadRegister();
			GetNextToken(Token.Type.CParen);
			return ret;
		}

		public T ParseNum<T>(Token? token = null) where T : IBinaryInteger<T>
		{
			token ??= Tokens.Dequeue();

			if (IsSymbolPass && token.Value.Symbol == Token.Type.Text) return T.AdditiveIdentity;
			else if (token.Value.Symbol == Token.Type.Text)
			{
				return GetVariable<T>(token.Value.Text);
			}
			else if (token.Value.Symbol == Token.Type.Minus)
			{
				return -ParseNum<T>(Tokens.Dequeue());
			}

			try
			{
				if (token.Value.Text.Contains("0x", StringComparison.CurrentCultureIgnoreCase)) return T.Parse(token.Value.Text[2..], System.Globalization.NumberStyles.HexNumber, null);
				if (token.Value.Text.Contains("0b", StringComparison.CurrentCultureIgnoreCase)) return T.Parse(token.Value.Text[2..], System.Globalization.NumberStyles.BinaryNumber, null);
				else return T.Parse(token.Value.Text, null);
			}
			catch (Exception)
			{
				throw new InvalidTokenException(token.Value, Token.Type.Integer);
			}
		}

		public static bool IsNum(Token token) => token.Symbol == Token.Type.Integer || token.Symbol == Token.Type.Text;

		public (byte type, byte[] data) ParsePointer(Token? token = null)
		{
			token ??= Tokens.Dequeue();
			Assume(token.Value, Token.Type.OSquareBracket);

			(byte type, byte[] data)? res;

			var first = Tokens.Dequeue();
			if (IsNum(first) && !(first.Text == "SP" || first.Text == "PC"))
			{
				var second = Tokens.Dequeue();
				if (second.Symbol == Token.Type.CSquareBracket) return (0b0010, Utils.ToBytes(ParseNum<uint>(first)));

				var third = Tokens.Dequeue();
				if (second.Symbol == Token.Type.Plus)
				{
					if (IsNum(third)) res = (0b0010, Utils.ToBytes(ParseNum<uint>(first) + ParseNum<short>(second)));
					else res = (0b0011, [.. Utils.ToBytes(ParseNum<uint>(first)), ParseRegister(third)]);
				}
				else if (second.Symbol == Token.Type.Minus) res = (0b0111, [.. Utils.ToBytes(ParseNum<uint>(first)), ParseRegister(third)]);
				else throw new InvalidTokenException(second);
			}
			else if (first.Symbol == Token.Type.Register || first.Text == "SP" || first.Text == "PC")
			{
				if (first.Symbol == Token.Type.Text || Tokens.Peek().Symbol == Token.Type.Comma)
				{
					var doubleReg = ParseDoubleRegister(first);
					var next = Tokens.Dequeue();

					if (next.Symbol == Token.Type.CSquareBracket) return (0b0110, [doubleReg]);

					var offset = Tokens.Dequeue();

					if (next.Symbol == Token.Type.Minus)
					{
						// I hate int
						if (IsNum(offset)) res = (0b0100, [doubleReg, .. Utils.ToBytes((short)-ParseNum<short>(offset))]);
						else if (offset.Symbol == Token.Type.Register) res = (0b1000, [doubleReg, ParseRegister(offset)]);
						else throw new InvalidTokenException(offset);
					}
					else if (next.Symbol == Token.Type.Plus)
					{
						if (IsNum(offset)) res = (0b0100, [doubleReg, .. Utils.ToBytes(ParseNum<short>(offset))]);
						else if (offset.Symbol == Token.Type.Register) res = (0b0101, [doubleReg, ParseRegister(offset)]);
						else throw new InvalidTokenException(offset);
					}
					else throw new InvalidTokenException(next);
				}
				else
				{
					var reg = ParseRegister(first);
					var next = Tokens.Dequeue();

					if (next.Symbol == Token.Type.CSquareBracket) return (0b1011, [reg]);

					var offset = Tokens.Dequeue();

					if (next.Symbol == Token.Type.Minus)
					{
						if (IsNum(offset)) res = (0b1001, [reg, .. Utils.ToBytes(-ParseNum<short>(offset))]);
						else if (offset.Symbol == Token.Type.Register) res = (0b1100, [Utils.Merge4Bit(reg, ParseRegister(offset))]);
						else throw new InvalidTokenException(offset);
					}
					else if (next.Symbol == Token.Type.Plus)
					{
						if (IsNum(offset)) res = (0b1001, [reg, .. Utils.ToBytes(ParseNum<short>(offset))]);
						else if (offset.Symbol == Token.Type.Register) res = (0b1010, [Utils.Merge4Bit(reg, ParseRegister(offset))]);
						else throw new InvalidTokenException(offset);
					}
					else throw new InvalidTokenException(next);
				}
			}
			else throw new InvalidTokenException(first);

			if (!res.HasValue) throw new InvalidTokenException(token.Value);
			GetNextToken(Token.Type.CSquareBracket);
			return res.Value;
		}

		public byte[] GetBinary()
		{
			var data = new byte[Addr - StartAddr];

			foreach (var i in Data)
			{
				i.Value.CopyTo(data, i.Key - StartAddr);
			}

			return data;
		}

		public void SetVariable<T>(string name, T value)
		{
			name = name.Replace(":", "");

			if (value is null) throw new ArgumentException("Value can't be null");

			if (!Variables.TryGetValue(typeof(T), out var vars))
			{
				Variables[typeof(T)] = [];
				vars = Variables[typeof(T)];
			}

			if (value is uint num)
			{
				if (num <= ushort.MaxValue) SetVariable(name, ushort.CreateTruncating(num));

				SetVariable(name + "#L", (ushort)(num & 0xFFFF));
				SetVariable(name + "#H", (ushort)(num >>> 16));
			}

			vars[name] = value;
		}

		public T GetVariable<T>(string name)
		{
			if (typeof(T) == typeof(ushort) && name == "HADDR") return (T)(object)ushort.CreateTruncating(Addr >>> 16);

			Variables.TryGetValue(typeof(T), out var vars);
			object? ret = null;
			vars?.TryGetValue(name, out ret);
			return (T)(ret ?? throw new ArgumentException($"Invalid variable {name} for type {typeof(T).Name}. Possible missing symbol?"));
		}

		public void SetVariablesForComputer(Computer computer)
		{
			foreach (var i in computer.GetComponents())
			{
				if (i is Terminal term) SetVariable("TERM_ADDR", term.Addr);
			}
		}
		
		// https://stackoverflow.com/questions/6629020/evaluate-escaped-string
		public static string ProcessString(string s)
		{
			StringBuilder sb = new();
			Regex r = new("\\\\[abfnrtv?\"'\\\\]|\\\\[0-3]?[0-7]{1,2}|\\\\u[0-9a-fA-F]{4}|\\\\U[0-9a-fA-F]{8}|.");
			MatchCollection mc = r.Matches(s, 0);

			foreach (Match m in mc)
			{
				if (m.Length == 1)
				{
					sb.Append(m.Value);
				}
				else
				{
					if (m.Value[1] >= '0' && m.Value[1] <= '7')
					{
						int i = Convert.ToInt32(m.Value.Substring(1), 8);
						sb.Append((char)i);
					}
					else if (m.Value[1] == 'u')
					{
						int i = Convert.ToInt32(m.Value.Substring(2), 16);
						sb.Append((char)i);
					}
					else if (m.Value[1] == 'U')
					{
						int i = Convert.ToInt32(m.Value.Substring(2), 16);
						sb.Append(char.ConvertFromUtf32(i));
					}
					else
					{
						switch (m.Value[1])
						{
							case 'a':
								sb.Append('\a');
								break;
							case 'b':
								sb.Append('\b');
								break;
							case 'f':
								sb.Append('\f');
								break;
							case 'n':
								sb.Append('\n');
								break;
							case 'r':
								sb.Append('\r');
								break;
							case 't':
								sb.Append('\t');
								break;
							case 'v':
								sb.Append('\v');
								break;
							default:
								sb.Append(m.Value[1]);
								break;
						}
					}
				}
			}

			return sb.ToString();
		}
	}
}
