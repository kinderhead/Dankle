using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	public interface IToken<T> where T : Enum
	{
		public T Symbol { get; }
		public int Index { get; }
		public string Text { get; }

		public int Line { get; }
		public int Column { get; }
	}

	public readonly struct Token(Token.Type symbol, int index, string text, int line, int column) : IToken<Token.Type>
	{
		public Type Symbol { get; } = symbol;
		public int Index { get; } = index;
		public string Text { get; } = text;

		public int Line { get; } = line;
		public int Column { get; } = column;

		public enum Type
		{
			Whitespace,
			Newline,
			Register,
			Integer,

			Export,

			Label,
			Text,
			Comma,
			OSquareBracket,
			CSquareBracket,
			OParam,
			CParam,
			Plus,
			Minus,
			String
		}
	}
}
