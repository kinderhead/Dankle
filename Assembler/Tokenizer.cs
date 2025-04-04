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
			return tk.Symbol != Token.Type.Whitespace && tk.Symbol != Token.Type.Newline && tk.Symbol != Token.Type.Comment;
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
			TokenMap[Token.Type.Comment] = new(@"^;.*\n?", RegexOptions.Compiled);
			TokenMap[Token.Type.Whitespace] = new(@"^( |\t|\r)+", RegexOptions.Compiled);
			TokenMap[Token.Type.Newline] = new(@"^\n+", RegexOptions.Compiled);
			TokenMap[Token.Type.Register] = new(@"^r([0-9]|1[0-5])\b", RegexOptions.Compiled);
			TokenMap[Token.Type.Integer] = new(@"^((0[bB][01]+)|(0[xX][0-9a-fA-F]+)|\d+)", RegexOptions.Compiled);
			TokenMap[Token.Type.Export] = new(@"^export", RegexOptions.Compiled);
			TokenMap[Token.Type.Import] = new(@"^import", RegexOptions.Compiled);
			TokenMap[Token.Type.Label] = new(@"^[a-zA-Z_$][a-zA-Z0-9_$#]*:", RegexOptions.Compiled);
			TokenMap[Token.Type.Text] = new(@"^[a-zA-Z_$][a-zA-Z0-9_$#]*", RegexOptions.Compiled);
			TokenMap[Token.Type.Comma] = new(@"^,", RegexOptions.Compiled);
			TokenMap[Token.Type.OSquareBracket] = new(@"^\[", RegexOptions.Compiled);
			TokenMap[Token.Type.CSquareBracket] = new(@"^\]", RegexOptions.Compiled);
			TokenMap[Token.Type.OParen] = new(@"^\(", RegexOptions.Compiled);
			TokenMap[Token.Type.CParen] = new(@"^\)", RegexOptions.Compiled);
			TokenMap[Token.Type.Plus] = new(@"^\+", RegexOptions.Compiled);
			TokenMap[Token.Type.Minus] = new(@"^\-", RegexOptions.Compiled);
			TokenMap[Token.Type.String] = new(@"^""[^""]+""", RegexOptions.Compiled);
		}

		public override Token MakeToken(Token.Type symbol, int index, string text, int line, int column) => new(symbol, index, text, line, column);
    }
}
