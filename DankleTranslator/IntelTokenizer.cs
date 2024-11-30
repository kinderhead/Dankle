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
			TokenMap[Token.Type.Register] = new(@"^(([a|b|c|d]x)|([d|e]s)|([s|b|i]p)|([s|d]i)|([a|b|c|d]l)|([a|b|c|d]h))\b");
			TokenMap[Token.Type.Integer] = new(@"^(([0-9a-fA-F]+H)|\d+)");

			TokenMap[Token.Type.Ignore] = new(@"^((\.387)|EXTRN|ASSUME|DGROUP|GROUP|(CONST\d*)|ENDS|DB|END|SEGMENT|WORD|USE16|BYTE|_DATA|offset DGROUP:|word ptr|dword ptr)");
			TokenMap[Token.Type.Public] = new(@"^PUBLIC");
			TokenMap[Token.Type.PtrLabel] = new(@"^es:");
			TokenMap[Token.Type.FakeLabel] = new(@"^[a-zA-Z_][a-zA-Z0-9_]*:[a-zA-Z_][a-zA-Z0-9_]*");
			TokenMap[Token.Type.BytePtr] = new(@"^byte ptr");

			TokenMap[Token.Type.SS] = new(@"^ss");
			TokenMap[Token.Type.CS] = new(@"^cs");

			TokenMap[Token.Type.Offset] = new(@"^offset");
			TokenMap[Token.Type.Seg] = new(@"^seg");

			TokenMap[Token.Type.NearPtr] = new(@"^near ptr");
			TokenMap[Token.Type.FarPtr] = new(@"^far ptr");

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
