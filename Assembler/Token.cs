using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	public readonly struct Token(Token.Type symbol, int index, string text, int line, int column)
	{
		public readonly Type Symbol = symbol;
		public readonly int Index = index;
		public readonly string Text = text;

		public readonly int Line = line;
		public readonly int Column = column;

		public enum Type
		{
			Whitespace,
			Newline,
			Instruction,
			Register,
			Integer,
			Label,
			Text,
			Comma,
			OSquareBracket,
			CSquareBracket,
			Plus,
			Minus
		}
	}
}
