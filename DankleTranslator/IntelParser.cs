using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Assembler;

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
                    ParseInsn(token);
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
        }

        private string lastLoadedLabel = "";
        private void ParseInsn(Token token)
        {
            var sig = GetNextInsn(token);

            if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tadd @1, %tmp, @1");
            else if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tadd @1, @2, @1");
            else if (sig.IsValid("sub", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tsub @1, %tmp, @1");
            else if (sig.IsValid("retf", [])) Output += "\tret";
            else if (sig.IsValid("retf", [ArgumentType.Integer]))
            {
                if (int.Parse(sig.Args[0].Item2) % 2 != 0) throw new Exception("WAH");
                for (var i = 0; i < int.Parse(sig.Args[0].Item2) / 2; i++)
                {
                    Output += sig.Compile("\npop %tmp\n");
                }
                Output += "\tret";
            }
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Label]))
            {
                Output += sig.Compile("\tld @1, @2#L\n");
                lastLoadedLabel = sig.Args[1].Item2;
                return;
            }
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.SS]))
            {
                if (lastLoadedLabel != "") Output += sig.Compile($"\tld @1, {lastLoadedLabel}#H");
                else Output += sig.Compile($"\tld @1, 0");
            }
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tmov @1, @2");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("\tld @1, @2");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld @1, @ptr2");
            else if (sig.IsValid("mov", [ArgumentType.Pointer, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tst @ptr1, %tmp");
            else if (sig.IsValid("mov", [ArgumentType.Pointer, ArgumentType.Register])) Output += sig.Compile("\tst @ptr1, @2");
            else if (sig.IsValid("mov", [ArgumentType.ByteRegister, ArgumentType.BytePointer])) Output += sig.Compile("\tldb @1, @ptr2");
            else if (sig.IsValid("mov", [ArgumentType.BytePointer, ArgumentType.ByteRegister])) Output += sig.Compile("\tstb @ptr1, @2");
            else if (sig.IsValid("lea", [ArgumentType.Register, ArgumentType.Pointer]))
            {
                if (sig.Args[1].Item2.Contains('-'))
                {
                    var reg = sig.Args[1].Item2.Split('-')[0];
                    var offset = sig.Args[1].Item2.Split('-')[1];
                    Output += $"\tld r11, {offset}\n\tsub {reg}, r11, {sig.Args[0].Item2}";
                }
                else throw new Exception("Invalid lea call");
            }
            else if (sig.IsValid("les", [ArgumentType.Register, ArgumentType.Pointer]))
            {
                if (sig.Args[1].Item2.Length >= 4) throw new Exception("Do this :(");
                else Output += sig.Compile("\tld @1, [%ds,@2+1]\n\tld %es, [%ds,@2]");
            }
            else if (sig.IsValid("push", [ArgumentType.Register])) Output += sig.Compile("\tpush @1");
            else if (sig.IsValid("push", [ArgumentType.CS])) return;
            else if (sig.IsValid("push", [ArgumentType.SS])) return;
            else if (sig.IsValid("pop", [ArgumentType.Register])) Output += sig.Compile("\tpop @1");
            else if (sig.IsValid("call", [ArgumentType.Label])) Output += sig.Compile("\tcall @1");
            else if (sig.IsValid("jmp", [ArgumentType.Label])) Output += sig.Compile("\tjmp @1");
            else if (sig.Name == "cmp")
            {
                var arg2 = "@2";
                if (sig.Args[1].Item1 == ArgumentType.Pointer)
                {
                    Output += sig.Compile($"\tld %tmp, @ptr2\n");
                    arg2 = "%tmp";
                }

                var cmp = GetNextInsn(Tokens.Dequeue());
                if (cmp.IsValid("jle", [ArgumentType.Label])) Output += sig.Compile($"\tlte @1, {arg2}") + cmp.Compile("\n\tje @1");
                else if (cmp.IsValid("jne", [ArgumentType.Label])) Output += sig.Compile($"\tcmp @1, {arg2}") + cmp.Compile("\n\tjne @1");
                else if (cmp.IsValid("jae", [ArgumentType.Label])) Output += sig.Compile($"\tgte @1, {arg2}") + cmp.Compile("\n\tje @1");
                else if (cmp.IsValid("jb", [ArgumentType.Label])) Output += sig.Compile($"\tlt @1, {arg2}") + cmp.Compile("\n\tje @1");
                else if (cmp.IsValid("ja", [ArgumentType.Label])) Output += sig.Compile($"\tgt @1, {arg2}") + cmp.Compile("\n\tje @1");
				else throw new Exception("Invalid cmp call");
            }
            else if (sig.IsValid("test", [ArgumentType.Register, ArgumentType.Register])) HandleTestInsn(sig);
            else if (sig.IsValid("test", [ArgumentType.ByteRegister, ArgumentType.ByteRegister])) HandleTestInsn(sig);
            else if (sig.IsValid("xor", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\txor @1, @2, @1");
            else if (sig.IsValid("xor", [ArgumentType.Label, ArgumentType.Label]))
            {
                if (sig.Args[0].Item2 != sig.Args[1].Item2) throw new Exception("Silly byte shennanigans");
                return;
            }
            else
            {
                Console.WriteLine(Output);
                throw new Exception("Invalid insn signature");
            }

            lastLoadedLabel = "";
            Output += "\n";
        }

        private void HandleTestInsn(InsnSignature sig)
        {
			var cmp = GetNextInsn(Tokens.Dequeue());
			if (cmp.IsValid("je", [ArgumentType.Label])) Output += sig.Compile("\tand @1, @2, %tmp") + cmp.Compile("\n\tjz @1");
			else throw new Exception("Invalid test call");
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
            else if (tok.Symbol == Token.Type.ByteRegister) return (ArgumentType.ByteRegister, MapRegister(tok.Text));
            else if (tok.Symbol == Token.Type.Integer) return (ArgumentType.Integer, ParseInt(tok));
            else if (tok.Symbol == Token.Type.Text) return (ArgumentType.Label, tok.Text);
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
            reg = reg.Replace('l', 'x');

            if (reg == "ax") return "r0";
            if (reg == "bx") return "r1";
            if (reg == "cx") return "r2";
            if (reg == "dx") return "r3";
            if (reg == "ds") return "r4";
            if (reg == "si") return "r5";
            if (reg == "bp") return "r6";
            if (reg == "di") return "r7";
            if (reg == "es") return "r8";
			if (reg == "sp") return "r13";

			throw new Exception($"Unmapped register {reg}");
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
