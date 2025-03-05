using System;
using System.Numerics;

namespace Assembler
{
    public abstract class BaseParser<TToken, TType>(List<TToken> tokens) where TType: struct, Enum where TToken : IToken<TType>
    {
#pragma warning disable IDE0306
		public Queue<TToken> Tokens { get; protected set; } = new(tokens);
#pragma warning restore IDE0306

		public abstract void Parse();

        public static TToken Assume(TToken token, TType expected)
		{
			if (!token.Symbol.Equals(expected)) throw new InvalidTokenException<TToken, TType>(token, expected);
			return token;
		}

        public TToken GetNextToken(TType expected) => Assume(Tokens.Dequeue(), expected);

        public bool TryGetToken(TType expected, out TToken token)
        {
            token = Tokens.Dequeue();
            return token.Symbol.Equals(expected);
        }
    }
}
