using System;
using Assembler;

namespace DankleTranslator
{
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
			
			Ignore,
			Public,
			FakeLabel,
			DB,

            NearPtr,
            FarPtr,

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
