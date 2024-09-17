using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public abstract class ArgumentParser(Parser parser)
	{
        public readonly Parser Parser = parser;

		/// <summary>
		/// Attempt to parse the instruction
		/// </summary>
		/// <returns>Argument type and optional extra data</returns>
		public abstract (byte type, byte[] data) Parse();
	}

	public class RegisterParser(Parser parser) : ArgumentParser(parser)
	{
		public override (byte type, byte[] data) Parse() => (Parser.ParseRegister(), []);
	}

	public class Any16NumParser(Parser parser) : ArgumentParser(parser)
	{
		public override (byte type, byte[] data) Parse()
		{
			var tok = Parser.Tokens.Dequeue();
			switch (tok.Symbol)
			{
				case Token.Type.Register:
					return (0b0001, [Parser.ParseRegister(tok)]);
				case Token.Type.Integer:
					break;
				case Token.Type.Label:
					break;
				case Token.Type.OSquareBracket:
					break;
				default:
					throw new InvalidTokenException(tok);
			}
		}
	}
}
