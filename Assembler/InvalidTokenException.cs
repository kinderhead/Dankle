using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	public class InvalidTokenException<TToken, TType> : Exception where TType: struct, Enum where TToken : IToken<TType>
	{
		public readonly TToken Token;
		public readonly TType? Expected = default;

		public InvalidTokenException(TToken tok)
		{
			Token = tok;
		}

		public InvalidTokenException(TToken tok, TType expected)
		{
			Token = tok;
			Expected = expected;
		}

		public override string Message => $"Invalid token \"{Token.Text}\" at ({Token.Line}:{Token.Column})" + (Expected is null ? "" : $". Expected \"{Enum.GetName(Expected.Value)}\"");

		public override string ToString()
		{
			return Message;
		}
	}

    public class InvalidTokenException : InvalidTokenException<Token, Token.Type>
    {
        public InvalidTokenException(Token tok) : base(tok)
        {
        }

        public InvalidTokenException(Token tok, Token.Type expected) : base(tok, expected)
        {
        }
    }
}
