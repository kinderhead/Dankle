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
			TokenMap[Token.Type.Comment] = new CommentToken();
			TokenMap[Token.Type.Whitespace] = new CollectiveOptionToken([' ', '\r', '\t']);
			TokenMap[Token.Type.Newline] = new CollectiveOptionToken(['\n']);
			TokenMap[Token.Type.Register] = new RegisterToken();
			TokenMap[Token.Type.Integer] = new IntegerToken();
			TokenMap[Token.Type.Export] = new ConstantToken("export");
			TokenMap[Token.Type.Import] = new ConstantToken("import");
			TokenMap[Token.Type.Label] = new LabelToken();
			TokenMap[Token.Type.Text] = new TextToken();
			TokenMap[Token.Type.Comma] = new ConstantToken(",");
			TokenMap[Token.Type.OSquareBracket] = new ConstantToken("[");
			TokenMap[Token.Type.CSquareBracket] = new ConstantToken("]");
			TokenMap[Token.Type.OParen] = new ConstantToken("(");
			TokenMap[Token.Type.CParen] = new ConstantToken(")");
			TokenMap[Token.Type.Plus] = new ConstantToken("+");
			TokenMap[Token.Type.Minus] = new ConstantToken("-");
			TokenMap[Token.Type.String] = new StringToken();
		}

		public override Token MakeToken(Token.Type symbol, int index, string text, int line, int column) => new(symbol, index, text, line, column);
    }
}
