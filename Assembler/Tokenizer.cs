using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler
{
	public class Tokenizer(string input)
	{
		public readonly string Input = input;

		private int Index;

		public List<Token> Parse()
		{
			var tokens = new List<Token>();

			while (Index < Input.Length)
			{
				var possibleTokens = new List<Token>();
				foreach (var i in TokenMap)
				{
					var match = i.Value.Match(Input[Index..]);
					if (match.Success)
					{
						var loc = GetLineAndColumn(Index);
						possibleTokens.Add(new(i.Key, Index, match.Value, loc.Item1, loc.Item2));
					}
				}

				if (possibleTokens.Count == 0) throw new Exception($"Invalid symbol at {GetLineAndColumn(Index)}: {Input[Index]}");

				var tk = possibleTokens.MaxBy(i => i.Text.Length);
				if (tk.Symbol != Token.Type.Whitespace && tk.Symbol != Token.Type.Newline) tokens.Add(tk);
				Index += tk.Text.Length;
			}

			return tokens;
		}

		public (int, int) GetLineAndColumn(int index)
		{
			var st = Input[..index];
			var line = st.Count(i => i == '\n');
			var col = st.Length - st.LastIndexOf('\n') - 1;
			return (line, col);
		}

		public static readonly Dictionary<Token.Type, Regex> TokenMap = [];

		static Tokenizer()
		{
			TokenMap[Token.Type.Whitespace] = new(@"^( |\t|\r)+");
			TokenMap[Token.Type.Newline] = new(@"^\n+");
			TokenMap[Token.Type.Register] = new(@"^r([0-9]|1[0-5])\b");
			TokenMap[Token.Type.Integer] = new(@"^((0[bB][01]+)|(0[xX][0-9a-fA-F]+)|\d+)");
			TokenMap[Token.Type.Label] = new(@"^[a-zA-Z_][a-zA-Z0-9_]+:");
			TokenMap[Token.Type.Text] = new(@"^[a-zA-Z_][a-zA-Z0-9_]+");
			TokenMap[Token.Type.Comma] = new(@"^,");
			TokenMap[Token.Type.OSquareBracket] = new(@"^\[");
			TokenMap[Token.Type.CSquareBracket] = new(@"^\]");
			TokenMap[Token.Type.OParam] = new(@"^\(");
			TokenMap[Token.Type.CParam] = new(@"^\)");
			TokenMap[Token.Type.Plus] = new(@"^\+");
			TokenMap[Token.Type.Minus] = new(@"^\-");
			TokenMap[Token.Type.String] = new(@"^""[^""]+""");
		}
	}
}
