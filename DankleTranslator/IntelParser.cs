using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Assembler;
using Dankle.Components;
using static System.Net.Mime.MediaTypeNames;

namespace DankleTranslator
{
    public class IntelParser(List<Token> tokens) : BaseParser<Token, Token.Type>(tokens)
    {
        public readonly List<string> PublicSymbols = [];

        public string Output { get; private set; } = "";
		public override void Parse()
        {
            while (Tokens.Count > 0)
            {
                var token = Tokens.Dequeue();

                HandleToken(token);
            }
        }

        private void HandleToken(Token token)
        {
			if (token.Symbol == Token.Type.Public)
			{
				if (TryGetToken(Token.Type.Text, out var sym))
				{
					PublicSymbols.Add(sym.Text);
					Output += $"export {sym.Text}\n";
				}
			}
			else if (token.Symbol == Token.Type.Text && token.Text.EndsWith("_TEXT"))
			{

			}
			else if (token.Symbol == Token.Type.Label)
			{
				if (Output.Length > 0 && Output.Last() != '\n') Output += "\n";
				Output += $"{token.Text}\n";
			}
			else if (token.Symbol == Token.Type.Text)
			{
				HandleInsn(GetNextInsn(token));
			}
			else if (token.Symbol == Token.Type.Integer)
			{
				if (Output.Last() != '\n') Output += " ";
				else Output += "\t";
				Output += $"{ParseInt(token)}";
			}
			else if (token.Symbol != Token.Type.Comma)
			{
				throw new InvalidTokenException<Token, Token.Type>(token);
			}
		}

        private void HandleInsn(InsnSignature sig)
        {
            Output += PrepareRegisters(sig);

            if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tadd @1, %tmp, @1");
            else if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr2\n\tadd @1, %tmp, @1");
            else if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tadd @1, @2, @1");
            else if (sig.IsValid("add", [ArgumentType.Pointer, ArgumentType.Integer])) Output += sig.Compile("\tld %tmp, @ptr1\n\tld %tmpalt, @2\n\tadd %tmp, %tmpalt, %tmp\n\tst @ptr1, %tmp");
            else if (sig.IsValid("adc", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tadc @1, @2, @1");
            else if (sig.IsValid("inc", [ArgumentType.Register])) Output += sig.Compile("\tinc @1");
            else if (sig.IsValid("inc", [ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr1\n\tinc %tmp\n\tst @ptr1, %tmp");
            else if (sig.IsValid("dec", [ArgumentType.Register])) Output += sig.Compile("\tdec @1");
            else if (sig.IsValid("dec", [ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr1\n\tdec %tmp\n\tst @ptr1, %tmp");
            else if (sig.IsValid("sub", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tsub @1, @2, @1");
            else if (sig.IsValid("sub", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tsub @1, %tmp, @1");
            else if (sig.IsValid("sub", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr2\n\tsub @1, %tmp, @1");
            else if (sig.IsValid("sbb", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tsbb @1, %tmp, @1");
            else if (sig.IsValid("sbb", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tsbb @1, @2, @1");
            else if (sig.IsValid("sbb", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr2\n\tsbb @1, %tmp, @1");
            else if (sig.IsValid("neg", [ArgumentType.Register])) Output += sig.Compile("\tneg @1");
            else if (sig.IsValid("neg", [ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr1\n\tneg %tmp\n\tst @ptr1, %tmp");
            else if (sig.IsValid("idiv", [ArgumentType.Register]))
            {
                if (sig.Args[0].Item2.Contains('%')) throw new NotImplementedException();

                Output += sig.Compile("\tmov %tmp, @1\n\tcall __IDIV");
                //Output += sig.Compile("\tld r9, 0\n\tsdivl (r3, r0), (r9, @1), (%tmp, %tmpalt)\n\tsmodl (r3, r0), (r9, @1), (%tmp, r3)\n\tmov r0, %tmpalt");
            }
            else if (sig.IsValid("ret", [])) Output += "\tpop r15";
            else if (sig.IsValid("ret", [ArgumentType.Integer])) Output += sig.Compile("\tld %tmp, @1\n\tadd r13, %tmp, r13\n\tpop r15");
            else if (sig.IsValid("retf", [])) Output += "\tret";
            else if (sig.IsValid("retf", [ArgumentType.Integer])) Output += sig.Compile("\tld %tmp, @1\n\tadd r13, %tmp, r13\n\tret");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Label]))
            {
                if (HasByteRegs(sig)) throw new NotImplementedException();

                var label = sig.Args[1].Item2;
                if (!label.Contains('#')) label += "#L";

                Output += sig.Compile($"\tld @1, {label}\n");
                if (Tokens.Peek().Symbol != Token.Type.Text) return;
                var next = GetNextInsn(Tokens.Dequeue());
                if (next.IsValid("mov", [ArgumentType.Register, ArgumentType.SS]))
                {
                    Output += next.Compile($"\tld @1, {sig.Args[1].Item2}#H\n");
                }
                else HandleInsn(next);
                return;
            }
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.SS])) Output += sig.Compile("\tld @1, HADDR");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tmov @1, @2");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("\tld @1, @2");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld @1, @ptr2");
            else if (sig.IsValid("mov", [ArgumentType.Pointer, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tst @ptr1, %tmp");
            else if (sig.IsValid("mov", [ArgumentType.BytePointer, ArgumentType.Integer])) Output += sig.Compile("\tldb %tmp, @2\n\tstb @ptr1, %tmp");
            else if (sig.IsValid("mov", [ArgumentType.Pointer, ArgumentType.Register])) Output += sig.Compile("\tst @ptr1, @2");
            else if (sig.IsValid("mov", [ArgumentType.Pointer, ArgumentType.Label])) Output += sig.Compile("%ldtmp2\tst @ptr1, %tmp");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.BytePointer])) Output += sig.Compile("\tldb @1, @ptr2");
            else if (sig.IsValid("mov", [ArgumentType.BytePointer, ArgumentType.Register])) Output += sig.Compile("\tstb @ptr1, @2");
            else if (sig.IsValid("lea", [ArgumentType.Register, ArgumentType.Pointer]))
            {
                if (sig.Args[1].Item2.Contains('-'))
                {
                    var reg = sig.Args[1].Item2.Split('-')[0];
                    var offset = sig.Args[1].Item2.Split('-')[1];
                    Output += $"\tld r11, {offset}\n\tsub {reg}, r11, {sig.Args[0].Item2}";
                }
                else if (sig.Args[1].Item2.Contains('+'))
                {
                    var reg = sig.Args[1].Item2.Split('+')[0];
                    var offset = sig.Args[1].Item2.Split('+')[1];
                    Output += $"\tld r11, {offset}\n\tadd {reg}, r11, {sig.Args[0].Item2}";
                }
                else throw new Exception("Invalid lea call");
            }
            else if (sig.IsValid("les", [ArgumentType.Register, ArgumentType.Pointer])) Output += LoadMem32Into("@1", "%es", sig.Args[1].Item2, sig);
            else if (sig.IsValid("lds", [ArgumentType.Register, ArgumentType.Pointer])) Output += LoadMem32Into("@1", "%ds", sig.Args[1].Item2, sig);
            else if (sig.IsValid("push", [ArgumentType.Register])) Output += sig.Compile("\tpush @1");
            else if (sig.IsValid("push", [ArgumentType.Pointer])) Output += sig.Compile("\tpush @ptr1");
            else if (sig.IsValid("push", [ArgumentType.CS])) return;
            else if (sig.IsValid("push", [ArgumentType.SS])) Output += sig.Compile("\tpush HADDR");
            else if (sig.IsValid("pop", [ArgumentType.Register])) Output += sig.Compile("\tpop @1");
            else if (sig.IsValid("cmp", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tsub @1, @2, %tmp");
            else if (sig.IsValid("cmp", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tsub @1, %tmp, %tmp");
            else if (sig.IsValid("cmp", [ArgumentType.Pointer, ArgumentType.Integer])) Output += sig.Compile("\tld %tmpalt, @ptr1\n%ldtmp2\tsub %tmpalt, %tmp, %tmp");
            else if (sig.IsValid("cmp", [ArgumentType.BytePointer, ArgumentType.Integer])) Output += sig.Compile("\tldb %tmpalt, @ptr1\n%ldtmp2\tsub %tmpalt, %tmp, %tmp");
            else if (sig.IsValid("cmp", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr2\n\tsub @1, %tmp, %tmp");
            else if (sig.IsValid("cmp", [ArgumentType.Register, ArgumentType.BytePointer])) Output += sig.Compile("\tldb %tmp, @ptr2\n\tsub @1, %tmp, %tmp");
			else if (sig.IsValid("je", [ArgumentType.Label])) Output += sig.Compile("\tjz @1");
            else if (sig.IsValid("jne", [ArgumentType.Label])) Output += sig.Compile("\tjnz @1");
            else if (sig.IsValid("jg", [ArgumentType.Label])) Output += sig.Compile("\tcmpfe 0b10001000\n\tcmpf 0b00110000, 0b00100000\n\tje @1");
            else if (sig.IsValid("jge", [ArgumentType.Label])) Output += sig.Compile("\tcmpfe 0b10001000\n\tje @1");
            else if (sig.IsValid("jle", [ArgumentType.Label])) Output += sig.Compile("\tcmpfo 0b10001000\n\tje @1\n\tjz @1");
            else if (sig.IsValid("jl", [ArgumentType.Label])) Output += sig.Compile("\tcmpfo 0b10001000\n\tje @1");
            else if (sig.IsValid("jae", [ArgumentType.Label])) Output += sig.Compile("\tcmpf 0b01000000, 0b00000000\n\tje @1");
            else if (sig.IsValid("ja", [ArgumentType.Label])) Output += sig.Compile("\tcmpf 0b01100000, 0b00000000\n\tje @1");
            else if (sig.IsValid("jb", [ArgumentType.Label])) Output += sig.Compile("\tcmpf 0b01000000, 0b01000000\n\tje @1");
            else if (sig.IsValid("jbe", [ArgumentType.Label])) Output += sig.Compile("\tcmpf 0b01000000, 0b01000000\n\tje @1\n\tjz @1");
            else if (sig.IsValid("call", [ArgumentType.FarLabel]))
            {
                if (sig.Args[0].Item2 == "__U4D") Output += sig.Compile("\tudivl (r3, r0), (r2, r1), (%tmp, %tmpalt)\n\tumodl (r3, r0), (r2, r1), (r2, r1)\n\tmov r3, %tmp\n\tmov r0, %tmpalt");
                else Output += sig.Compile("\tcall @1");
            }
            else if (sig.IsValid("call", [ArgumentType.NearLabel])) Output += sig.Compile("\txcall @1");
            else if (sig.IsValid("call", [ArgumentType.Pointer])) Output += sig.Compile($"{LoadMem32Into("%tmp", "%tmpalt", sig.Args[0].Item2, sig)}\n\tcall (%tmpalt,%tmp)");
            else if (sig.IsValid("jmp", [ArgumentType.Label])) Output += sig.Compile("\tjmp @1");
            else if (sig.IsValid("jmp", [ArgumentType.NearLabel])) Output += sig.Compile("\tjmp @1");
			else if (sig.IsValid("test", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tand @1, @2, %tmp");
            else if (sig.IsValid("test", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tand %tmp, %tmp, %tmp");
            else if (sig.IsValid("test", [ArgumentType.BytePointer, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tldb %tmpalt, @ptr1\n\tand %tmp, %tmpalt, %tmp");
            else if (sig.IsValid("and", [ArgumentType.BytePointer, ArgumentType.Integer])) Output += sig.Compile("\tldb %tmp, @ptr1\n\tld %tmpalt, @2\n\tand %tmp, %tmpalt, %tmp\n\tstb @ptr1, %tmp");
            else if (sig.IsValid("or", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tor @1, @2, @1");
            else if (sig.IsValid("or", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tor @1, %tmp, @1");
            else if (sig.IsValid("or", [ArgumentType.BytePointer, ArgumentType.Integer])) Output += sig.Compile("\tldb %tmp, @ptr1\n\tld %tmpalt, @2\n\tor %tmp, %tmpalt, %tmp\n\tstb @ptr1, %tmp");
            else if (sig.IsValid("xor", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\txor @1, @2, @1");
            else if (sig.IsValid("shl", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tlsh @1, %tmp, @1");
            else if (sig.IsValid("cwd", [])) Output += sig.Compile("\tcall __CWD");
            else
            {
                Console.WriteLine(Output);
                throw new Exception("Invalid insn signature");
            }

            Output += "\n";
            Output += RestoreRegisters(sig);
        }

        private static string PrepareRegisters(InsnSignature sig)
        {
            var ret = "";

            var modifiedRegs = new Dictionary<char, CPUCore.RegisterHandler.RegisterState>();
			foreach (var i in sig.Args)
			{
                if (i.Item1 == ArgumentType.Register && i.Item2.EndsWith('h'))
                {
                    if (modifiedRegs.TryGetValue(i.Item2[1], out var state))
                    {
                        if (state != CPUCore.RegisterHandler.RegisterState.High) throw new Exception("Uh oh");
                        else continue;
                    }
                    modifiedRegs[i.Item2[1]] = CPUCore.RegisterHandler.RegisterState.High;
                    ret += $"\thigh {i.Item2.Split("%")[0]}\n";
                }
                else if (i.Item1 == ArgumentType.Register && i.Item2.EndsWith('l'))
				{
					if (modifiedRegs.TryGetValue(i.Item2[1], out var state))
					{
						if (state != CPUCore.RegisterHandler.RegisterState.Low) throw new Exception("Uh oh");
						else continue;
					}
					modifiedRegs[i.Item2[1]] = CPUCore.RegisterHandler.RegisterState.Low;
					ret += $"\tlow {i.Item2.Split("%")[0]}\n";
				}
			}

			foreach (var i in sig.Args)
            {
                if (i.Item1 == ArgumentType.Pointer)
                {
                    foreach (var e in modifiedRegs.Keys)
                    {
                        if (i.Item2.Contains(e)) throw new Exception("WOAH!!!!");
                    }
                }
            }

			return ret;
		}

        private static bool HasByteRegs(InsnSignature sig)
        {
			foreach (var i in sig.Args)
			{
                if (i.Item1 == ArgumentType.Register && (i.Item2.EndsWith('h') || i.Item2.EndsWith('l'))) return true;
			}
            return false;
		}

        private static string RestoreRegisters(InsnSignature sig)
        {
			var ret = "";
            List<string> restored = [];
			foreach (var i in sig.Args)
			{
                if (i.Item1 == ArgumentType.Register && (i.Item2.EndsWith('h') || i.Item2.EndsWith('l')) && !restored.Contains(i.Item2))
                {
                    ret += $"\trst {i.Item2.Split("%")[0]}\n";
                    restored.Add(i.Item2);
                }
			}
			return ret;
		}

        private static string LoadMem32Into(string high, string low, string ptr, InsnSignature sig)
        {
			var offset = 0;

			if (ptr.Contains('+'))
			{
				offset = Convert.ToInt16(ptr.Split('+').Last(), 16);
				ptr = ptr.Split('+').First();
			}
			else if (ptr.Contains('-'))
			{
				offset = -Convert.ToInt16(ptr.Split('-').Last(), 16);
				ptr = ptr.Split('-').First();
			}

			return sig.Compile($"\tld {high}, [{InsnSignature.GetIndirectHighReg(ptr)},{ptr}{ToPointerOffset(offset)}]\n\tld {low}, [{InsnSignature.GetIndirectHighReg(ptr)},{ptr}{ToPointerOffset(offset + 2)}]");
		}

        private InsnSignature GetNextInsn(Token first)
        {
            Assume(first, Token.Type.Text);
            var name = first.Text;

            var next = Tokens.Peek();
            if (next.Line != first.Line) return new(name, []);

            List<(ArgumentType, string)> args = [];
            while (true)
            {
                args.Add(ParseArg());
                if (Tokens.Peek().Symbol == Token.Type.Comma)
                {
                    GetNextToken(Token.Type.Comma);
                }
                else break;
            }
            return new(name, args);
        }

        private (ArgumentType, string) ParseArg(Token? token = null)
        {
            var tok = token ?? Tokens.Dequeue();

            if (tok.Symbol == Token.Type.Register) return (ArgumentType.Register, MapRegister(tok.Text));
			else if (tok.Symbol == Token.Type.Integer) return (ArgumentType.Integer, ParseInt(tok));
            else if (tok.Symbol == Token.Type.Text) return (ArgumentType.Label, tok.Text);
			else if (tok.Symbol == Token.Type.NearPtr) return (ArgumentType.NearLabel, GetNextToken(Token.Type.Text).Text);
			else if (tok.Symbol == Token.Type.FarPtr) return (ArgumentType.FarLabel, GetNextToken(Token.Type.Text).Text);
			else if (tok.Symbol == Token.Type.Offset) return (ArgumentType.Label, GetNextToken(Token.Type.Text).Text + "#L");
            else if (tok.Symbol == Token.Type.Seg) return (ArgumentType.Label, GetNextToken(Token.Type.Text).Text + "#H");
            else if (tok.Symbol == Token.Type.SS) return (ArgumentType.SS, "ss");
            else if (tok.Symbol == Token.Type.CS) return (ArgumentType.CS, "cs");
            else if (tok.Symbol == Token.Type.OSquareBracket || tok.Symbol == Token.Type.PtrLabel) return (ArgumentType.Pointer, ParsePointer(tok));
            else if (tok.Symbol == Token.Type.BytePtr) return (ArgumentType.BytePointer, ParsePointer(Tokens.Dequeue()));
            else Err(tok);
            return (ArgumentType.Label, "");
        }

        private string ParsePointer(Token tok)
        {
			var next = Tokens.Dequeue();
            var ret = "";

            if (next.Symbol == Token.Type.Register)
            {
                ret = MapRegister(next.Text);

                if (Tokens.Peek().Symbol == Token.Type.Minus)
                {
                    Tokens.Dequeue();
                    ret += $"-{ParseInt(GetNextToken(Token.Type.Integer))}";
                }
                else if (Tokens.Peek().Symbol == Token.Type.Plus)
				{
					Tokens.Dequeue();
					ret += $"+{ParseInt(GetNextToken(Token.Type.Integer))}";
				}
			}
            else if (tok.Symbol == Token.Type.PtrLabel)
            {
                var offset = MapRegister(tok.Text.Trim(':'));
                return $"{offset},{ParsePointer(next)}";
			}
            else Err(tok);

			GetNextToken(Token.Type.CSquareBracket);
			return ret;
		}

        private static string ParseInt(Token token)
        {
            Assume(token, Token.Type.Integer);

            if (token.Text.EndsWith('H')) return $"0x{token.Text.Trim('H').TrimStart('0')}";
            else return token.Text;
        }

        private static string MapRegister(string reg)
        {
            var postfix = "";

            if (reg[1] == 'l') postfix = "%l";
            else if (reg[1] == 'h') postfix = "%h";
			reg = reg.Replace('l', 'x');
            reg = reg.Replace('h', 'x');

			if (reg == "ax") return $"r0{postfix}";
            if (reg == "bx") return $"r1{postfix}";
            if (reg == "cx") return $"r2{postfix}";
            if (reg == "dx") return $"r3{postfix}";
            if (reg == "ds") return "r4";
            if (reg == "si") return "r5";
            if (reg == "bp") return "r6";
            if (reg == "di") return "r7";
            if (reg == "es") return "r8";
			if (reg == "sp") return "r13";

			throw new Exception($"Unmapped register {reg}");
        }

        private static string ToPointerOffset(int offset)
        {
            if (offset == 0) return "";
            else if (offset > 0) return $"+{offset}";
            return $"{offset}";
        }

        private static void Err(Token tk)
        {
            throw new InvalidTokenException<Token, Token.Type>(tk);
        }

        //private static void Err(Token tk, Token.Type expected)
        //{
        //    throw new InvalidTokenException<Token, Token.Type>(tk, expected);
        //}
    }
}
