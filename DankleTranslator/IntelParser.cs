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
            else if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr2\n\tadd @1, %tmp, @1");
            else if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tadd @1, @2, @1");
            else if (sig.IsValid("inc", [ArgumentType.Register])) Output += sig.Compile("\tinc @1");
            else if (sig.IsValid("inc", [ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr1\n\tinc %tmp\n\tst @ptr1, %tmp");
            else if (sig.IsValid("dec", [ArgumentType.Register])) Output += sig.Compile("\tdec @1");
            else if (sig.IsValid("dec", [ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr1\n\tdec %tmp\n\tst @ptr1, %tmp");
            else if (sig.IsValid("sub", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tsub @1, %tmp, @1");
            else if (sig.IsValid("sub", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr2\n\tsub @1, %tmp, @1");
            else if (sig.IsValid("ret", [])) Output += "\tret";
            else if (sig.IsValid("ret", [ArgumentType.Integer])) Output += sig.Compile("\tld %tmp, @1\n\tadd r13, %tmp, r13\n\tret");
            else if (sig.IsValid("retf", [])) Output += "\tret";
            else if (sig.IsValid("retf", [ArgumentType.Integer])) Output += sig.Compile("\tld %tmp, @1\n\tadd r13, %tmp, r13\n\tret");
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
            else if (sig.IsValid("mov", [ArgumentType.ByteRegister, ArgumentType.ByteRegister])) Output += sig.Compile("\tmov @1, @2");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("\tld @1, @2");
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Pointer])) Output += sig.Compile("\tld @1, @ptr2");
            else if (sig.IsValid("mov", [ArgumentType.Pointer, ArgumentType.Integer])) Output += sig.Compile("%ldtmp2\tst @ptr1, %tmp");
            else if (sig.IsValid("mov", [ArgumentType.BytePointer, ArgumentType.Integer])) Output += sig.Compile("\tldb %tmp, @2\n\tstb @ptr1, %tmp");
            else if (sig.IsValid("mov", [ArgumentType.Pointer, ArgumentType.Register])) Output += sig.Compile("\tst @ptr1, @2");
            else if (sig.IsValid("mov", [ArgumentType.Pointer, ArgumentType.Label])) Output += sig.Compile("%ldtmp2\tst @ptr1, %tmp");
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
                else if (sig.Args[1].Item2.Contains('+'))
                {
                    var reg = sig.Args[1].Item2.Split('+')[0];
                    var offset = sig.Args[1].Item2.Split('+')[1];
                    Output += $"\tld r11, {offset}\n\tadd {reg}, r11, {sig.Args[0].Item2}";
                }
                else throw new Exception("Invalid lea call");
            }
            else if (sig.IsValid("les", [ArgumentType.Register, ArgumentType.Pointer]))
            {
                if (sig.Args[1].Item2.Length >= 4) throw new Exception("Do this :(");
                else Output += sig.Compile($"\tld @1, [{InsnSignature.GetIndirectHighReg(sig.Args[0].Item2)},@2+1]\n\tld %es, [{InsnSignature.GetIndirectHighReg(sig.Args[0].Item2)},@2]");
            }
            else if (sig.IsValid("push", [ArgumentType.Register])) Output += sig.Compile("\tpush @1");
            else if (sig.IsValid("push", [ArgumentType.Pointer])) Output += sig.Compile("\tld %tmp, @ptr1\n\tpush %tmp");
            else if (sig.IsValid("push", [ArgumentType.CS])) return;
            else if (sig.IsValid("push", [ArgumentType.SS])) return;
            else if (sig.IsValid("pop", [ArgumentType.Register])) Output += sig.Compile("\tpop @1");
            else if (sig.IsValid("je", [ArgumentType.Label])) Output += sig.Compile("\tjz @1");
            else if (sig.IsValid("jne", [ArgumentType.Label])) Output += sig.Compile("\tjnz @1");
            else if (sig.IsValid("call", [ArgumentType.Label])) Output += sig.Compile("\tcall @1");
            else if (sig.IsValid("call", [ArgumentType.Pointer]))
            {
                if (sig.Args[0].Item2.Length >= 4)
                {
                    int offset;
                    string reg;
                    if (sig.Args[0].Item2.Contains('-'))
                    {
                        offset = -Convert.ToInt32(sig.Args[0].Item2.Split('-')[1], 16);
                        reg = sig.Args[0].Item2.Split('-')[0];
                    }
                    else if (sig.Args[0].Item2.Contains('+'))
                    {
                        offset = Convert.ToInt32(sig.Args[0].Item2.Split('+')[1], 16);
                        reg = sig.Args[0].Item2.Split('+')[0];
                    }
                    else throw new Exception("Huh?");

                    var high = InsnSignature.GetIndirectHighReg(reg);

                    Output += sig.Compile($"\tld %tmp, [{high},{reg}{(offset >= 0 ? "+" : "")}{offset}]\n\tld %tmpalt, [{high},{reg}{((offset+2) >= 0 ? "+" : "")}{offset+2}]\n\tcall [%tmpalt,%tmp]");
                }
                else throw new Exception("Do this :(");
            }
            else if (sig.IsValid("jmp", [ArgumentType.Label])) Output += sig.Compile("\tjmp @1");
            else if (sig.Name == "cmp")
            {
                var arg1 = "@1";
                if (sig.Args[0].Item1 == ArgumentType.Pointer)
                {
                    arg1 = "@ptr1";
                }

                var arg2 = "@2";
                if (sig.Args[1].Item1 == ArgumentType.Pointer)
                {
                    arg2 = "@ptr2";
                }

                var cmp = GetNextInsn(Tokens.Dequeue());
                if (cmp.IsValid("jle", [ArgumentType.Label])) Output += sig.Compile($"\tlte {arg1}, {arg2}") + cmp.Compile("\n\tje @1");
                else if (cmp.IsValid("jne", [ArgumentType.Label])) Output += sig.Compile($"\tcmp {arg1}, {arg2}") + cmp.Compile("\n\tjne @1");
                else if (cmp.IsValid("je", [ArgumentType.Label])) Output += sig.Compile($"\tcmp {arg1}, {arg2}") + cmp.Compile("\n\tje @1");
                else if (cmp.IsValid("jae", [ArgumentType.Label])) Output += sig.Compile($"\tgte {arg1}, {arg2}") + cmp.Compile("\n\tje @1");
                else if (cmp.IsValid("jb", [ArgumentType.Label])) Output += sig.Compile($"\tlt {arg1}, {arg2}") + cmp.Compile("\n\tje @1");
                else if (cmp.IsValid("ja", [ArgumentType.Label])) Output += sig.Compile($"\tgt {arg1}, {arg2}") + cmp.Compile("\n\tje @1");
				else throw new Exception("Invalid cmp call");
            }
            else if (sig.IsValid("test", [ArgumentType.Register, ArgumentType.Register])) HandleTestInsn(sig);
            else if (sig.IsValid("test", [ArgumentType.BytePointer, ArgumentType.Integer])) HandleTestInsn(sig);
            else if (sig.IsValid("test", [ArgumentType.ByteRegister, ArgumentType.ByteRegister])) HandleTestInsn(sig);
            else if (sig.IsValid("test", [ArgumentType.ByteRegister, ArgumentType.Integer])) HandleTestInsn(sig);
            else if (sig.IsValid("and", [ArgumentType.BytePointer, ArgumentType.Integer])) Output += sig.Compile("\tldb %tmp, @ptr1\n\tand %tmp, @2, %tmp\n\tstb @ptr1, %tmp");
            else if (sig.IsValid("or", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\tor @1, @2, @1");
            else if (sig.IsValid("xor", [ArgumentType.Register, ArgumentType.Register])) Output += sig.Compile("\txor @1, @2, @1");
            else if (sig.IsValid("xor", [ArgumentType.Label, ArgumentType.Label]))
            {
                if (sig.Args[0].Item2 != sig.Args[1].Item2) throw new Exception("Silly byte shennanigans");
                return;
            }
            else if (sig.IsValid("shl", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("\tshl @1, @2, @1");
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
            var arg1 = "@1";
            var arg2 = "@2";

            if (sig.Args[0].Item1 == ArgumentType.Pointer) arg1 = "@ptr1";
            else if (sig.Args[0].Item1 == ArgumentType.BytePointer)
            {
                Output += sig.Compile("\tldb %tmp, @ptr1\n");
                arg1 = "%tmp";
            }
            if (sig.Args[1].Item1 == ArgumentType.Pointer) arg2 = "@ptr2";
            else if (sig.Args[1].Item1 == ArgumentType.BytePointer)
            {
                Output += sig.Compile("\tldb %tmpalt, @ptr2\n");
                arg1 = "%tmpalt";
            }

			if (cmp.IsValid("je", [ArgumentType.Label])) Output += sig.Compile($"\tcmp {arg1}, {arg2}") + cmp.Compile("\n\tje @1");
			else if (cmp.IsValid("jne", [ArgumentType.Label])) Output += sig.Compile($"\tcmp {arg1}, {arg2}") + cmp.Compile("\n\tjne @1");
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
