using System;
using System.Numerics;

namespace Assembler
{
    public abstract class BaseParser<TToken, TType>(List<TToken> tokens) where TType: struct, Enum where TToken : IToken<TType>
    {
        public Queue<TToken> Tokens { get; protected set; } = new(tokens);

        public abstract void Parse();

        public static TToken Assume(TToken token, TType expected)
		{
            // Silly C# comparison doesn't work
			if (!token.Symbol.Equals(expected)) throw new InvalidTokenException<TToken, TType>(token, expected);
			return token;
		}

        public TToken GetNextToken(TType expected) => Assume(Tokens.Dequeue(), expected);
    }
}
