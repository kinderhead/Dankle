using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler
{
	public class Tokenizer : BaseTokenizer<Token, Token.Type>
    {
		protected override bool KeepToken(Token tk)
		{
			return tk.Symbol != Token.Type.Whitespace && tk.Symbol != Token.Type.Newline;
		}

        public Tokenizer(string input, bool includeBIOS = true) : base(input)
        {
			if (includeBIOS)
			{
				Input += "\n" + Encoding.Default.GetString(File.ReadAllBytes(Path.GetDirectoryName(GetType().Assembly.Location) + "/bios.asm"));
			}
        }

        protected override void GenerateTokenMap()
		{
			TokenMap[Token.Type.Whitespace] = new(@"^( |\t|\r)+");
			TokenMap[Token.Type.Newline] = new(@"^\n+");
			TokenMap[Token.Type.Register] = new(@"^r([0-9]|1[0-5])\b");
			TokenMap[Token.Type.Integer] = new(@"^((0[bB][01]+)|(0[xX][0-9a-fA-F]+)|\d+)");
			TokenMap[Token.Type.Export] = new(@"^export");
			TokenMap[Token.Type.Label] = new(@"^[a-zA-Z_$][a-zA-Z0-9_$#]*:");
			TokenMap[Token.Type.Text] = new(@"^[a-zA-Z_$][a-zA-Z0-9_$#]*");
			TokenMap[Token.Type.Comma] = new(@"^,");
			TokenMap[Token.Type.OSquareBracket] = new(@"^\[");
			TokenMap[Token.Type.CSquareBracket] = new(@"^\]");
			TokenMap[Token.Type.OParam] = new(@"^\(");
			TokenMap[Token.Type.CParam] = new(@"^\)");
			TokenMap[Token.Type.Plus] = new(@"^\+");
			TokenMap[Token.Type.Minus] = new(@"^\-");
			TokenMap[Token.Type.String] = new(@"^""[^""]+""");
		}

		public override Token MakeToken(Token.Type symbol, int index, string text, int line, int column) => new(symbol, index, text, line, column);
    }
}
