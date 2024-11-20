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
                    if (TryGetToken(Token.Type.Text, out var sym)) PublicSymbols.Add(sym.Text);
                }
                else if (token.Symbol == Token.Type.Text && token.Text.EndsWith("_TEXT"))
                {
                    
                }
                else if (token.Symbol == Token.Type.Label)
                {
                    Output += $"{token.Text}\n";
                }
                else if (token.Symbol == Token.Type.Text)
                {
                    ParseInsn(token);
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

            if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("%ldtmp@2\tadd @1, @1, %tmp");
            else if (sig.IsValid("retf", [])) Output += "\tret";
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.Label]))
            {
                Output += sig.Compile("\tld @1, @2#L");
                lastLoadedLabel = sig.Args[1].Item2;
            }
            else if (sig.IsValid("mov", [ArgumentType.Register, ArgumentType.SS])) Output += sig.Compile($"\tld @1, {lastLoadedLabel}#H");
            else if (sig.IsValid("push", [ArgumentType.Register])) Output += sig.Compile("\tpush @1");
            else if (sig.IsValid("push", [ArgumentType.CS])) return;
            else if (sig.IsValid("call", [ArgumentType.Label])) Output += sig.Compile("\tcall @1");
            else throw new Exception("Invalid insn signature");

            Output += "\n";
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

            if (tok.Symbol == Token.Type.Register) return (ArgumentType.Register, $"{MapRegister(tok.Text)}");
            else if (tok.Symbol == Token.Type.Integer) return (ArgumentType.Integer, tok.Text);
            else if (tok.Symbol == Token.Type.Text) return (ArgumentType.Label, tok.Text);
            else if (tok.Symbol == Token.Type.SS) return (ArgumentType.SS, "ss");
            else if (tok.Symbol == Token.Type.CS) return (ArgumentType.CS, "cs");
			else Err(tok);
            return (ArgumentType.Label, "");
        }

        private static string MapRegister(string reg)
        {
            if (reg == "ax") return "r0";
            if (reg == "bx") return "r1";
            if (reg == "cx") return "r2";
            if (reg == "dx") return "r3";

            throw new Exception($"Unmapped register {reg}");
        }

        private static void Err(Token tk)
        {
            throw new InvalidTokenException<Token, Token.Type>(tk);
        }
    }
}
