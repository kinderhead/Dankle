using System;
using Assembler;

namespace DankleTranslator
{
    public class IntelParser(List<Token> tokens) : BaseParser<Token, Token.Type>(tokens)
    {
        public readonly List<string> PublicSymbols = [];

        public string Output { get; private set; } = "";

        private bool startedAssembly = false;

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
                    startedAssembly = true;
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

        private void ParseInsn(Token token)
        {
            Output += "\t";
            switch (token.Text)
            {
                case "mov":
                    Output += "mov ";

                    var movSrc = Tokens.Dequeue();
                    if (movSrc.Symbol == Token.Type.Register) Output += $"{MapRegister(movSrc.Text)}, ";
                    else Err(movSrc);

                    GetNextToken(Token.Type.Comma);
                    var movDest = Tokens.Dequeue();
                    if (movDest.Symbol == Token.Type.Register) Output += $"{MapRegister(movDest.Text)}";
                    else if (movDest.Symbol == Token.Type.Integer) Output += movDest.Text;
                    else Err(movDest);
                    break;
                case "retf":
                case "retn":
                    Output += "ret";
                    break;
                default:
                    throw new Exception($"Invalid instruction {token.Text}");
            }
            Output += "\n";
        }

        private static string MapRegister(string reg)
        {
            if (reg == "ax") return "r0";
            if (reg == "bx") return "r1";
            if (reg == "cx") return "r2";
            if (reg == "dx") return "r3";

            throw new Exception($"Unmapped register {reg}");
        }

        private void Err(Token tk)
        {
            throw new InvalidTokenException<Token, Token.Type>(tk);
        }
    }
}
