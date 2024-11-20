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
			TokenMap[Token.Type.Register] = new(@"^(([a|b|c|d]x)|([c|d|s|e]s)|([s|b|i]p)|([s|d]i))\b");
			TokenMap[Token.Type.Integer] = new(@"^(([0-9a-fA-F]+H)|\d+)");

			TokenMap[Token.Type.Ignore] = new(@"^((\.387)|EXTRN|ASSUME|DGROUP|GROUP|(CONST\d*)|ENDS|SEGMENT|WORD|USE16|BYTE|_DATA|offset DGROUP:)");
			TokenMap[Token.Type.Public] = new(@"^PUBLIC");
			TokenMap[Token.Type.FakeLabel] = new(@"^[a-zA-Z_][a-zA-Z0-9_]*:[a-zA-Z_][a-zA-Z0-9_]*");
			TokenMap[Token.Type.DB] = new(@"^DB");

			TokenMap[Token.Type.FarPtr] = new(@"^far ptr");
			TokenMap[Token.Type.NearPtr] = new(@"^near ptr");

			TokenMap[Token.Type.Label] = new(@"^[a-zA-Z_$][a-zA-Z0-9_$]+:");
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

        protected override bool KeepToken(Token tk)
        {
			List<Token.Type> skip = [Token.Type.Whitespace, Token.Type.Newline, Token.Type.Ignore, Token.Type.FakeLabel];

			return !skip.Contains(tk.Symbol);
        }
    }
}
