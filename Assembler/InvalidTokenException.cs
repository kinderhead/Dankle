using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
	public class InvalidTokenException : Exception
	{
		public readonly Token Token;
		public readonly Token.Type? Expected = null;

		public InvalidTokenException(Token tok)
		{
			Token = tok;
		}

		public InvalidTokenException(Token tok, Token.Type expected)
		{
			Token = tok;
			Expected = expected;
		}

		public override string Message => $"Invalid token \"{Token.Text}\" at ({Token.Line}:{Token.Column})" + (Expected is null ? "" : $". Expected \"{Enum.GetName(Expected.Value)}\"");
	}
}
