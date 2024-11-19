using System;
using Assembler;

namespace DankleTranslator
{
    public class IntelTokenizer(string input) : BaseTokenizer<Token, Token.Type>(input)
    {
        public override Token MakeToken(Token.Type symbol, int index, string text, int line, int column) => new(symbol, index, text, line, column);

        protected override void GenerateTokenMap()
        {
            TokenMap[Token.Type.Whitespace] = new(@"^( |\t|\r)+");
			TokenMap[Token.Type.Newline] = new(@"^\n+");
			TokenMap[Token.Type.Register] = new(@"^r([0-9]|1[0-5])\b");
			TokenMap[Token.Type.Integer] = new(@"^((0[bB][01]+)|(0[xX][0-9a-fA-F]+)|\d+)");

			TokenMap[Token.Type.Ignore] = new(@"^(\.387)|EXTRN|DGROUP|GROUP|(CONST\d*)|ENDS|SEGMENT|WORD|USE16|BYTE|_DATA");
			TokenMap[Token.Type.Public] = new(@"PUBLIC");
			TokenMap[Token.Type.FakeLabel] = new(@"^[a-zA-Z_][a-zA-Z0-9_]*:[a-zA-Z_][a-zA-Z0-9_]*");

			TokenMap[Token.Type.FarPtr] = new(@"^far ptr");
			TokenMap[Token.Type.NearPtr] = new(@"^near ptr");

			TokenMap[Token.Type.Label] = new(@"^[a-zA-Z_][a-zA-Z0-9_]+:");
			TokenMap[Token.Type.Text] = new(@"^[a-zA-Z_$][a-zA-Z0-9_$]+");
			TokenMap[Token.Type.Comma] = new(@"^,");
			TokenMap[Token.Type.OSquareBracket] = new(@"^\[");
			TokenMap[Token.Type.CSquareBracket] = new(@"^\]");
			TokenMap[Token.Type.OParam] = new(@"^\(");
			TokenMap[Token.Type.CParam] = new(@"^\)");
			TokenMap[Token.Type.Plus] = new(@"^\+");
			TokenMap[Token.Type.Minus] = new(@"^\-");
			TokenMap[Token.Type.String] = new(@"^'[^']+'");
        }
    }
}
