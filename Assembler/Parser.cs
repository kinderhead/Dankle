using Dankle.Components.Arguments;
using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class Parser
	{
		public readonly Queue<Token> Tokens;
		
        public readonly Dictionary<string, int> Labels = [];

		private readonly Dictionary<Type, ArgumentParser> ArgParsers = [];

		public Parser(List<Token> tokens)
		{
			Tokens = new(tokens);

			ArgParsers[typeof(Register)] = new RegisterParser(this);
		}

		public void Parse()
        {
            while (Tokens.Count > 0)
            {
                var token = Tokens.Dequeue();

                if (token.Symbol == Token.Type.Instruction)
                {
                    var insn = Instruction.Get(token.Text);

					List<byte> argTypes = [];
					List<byte> argData = [];

					foreach (var i in insn.Arguments)
					{
						var (type, data) = ArgParsers[i].Parse();
						argTypes.Add(type);
						argData.AddRange(data);
					}
				}
            }
        }

		public byte ParseRegister(Token? token = null)
		{
			token ??= Tokens.Dequeue();
			if (token.Value.Symbol != Token.Type.Register) throw new InvalidTokenException(token.Value, Token.Type.Register);
			return byte.Parse(token.Value.Text[1..]);
		}

		public T ParseNum<T>(Token? token = null) where T : IBinaryInteger<T>
		{
			token ??= Tokens.Dequeue();

			try
			{
				if (token.Value.Text.Contains("0x", StringComparison.CurrentCultureIgnoreCase)) return T.Parse(token.Value.Text[..2], System.Globalization.NumberStyles.HexNumber, null);
				if (token.Value.Text.Contains("0b", StringComparison.CurrentCultureIgnoreCase)) return T.Parse(token.Value.Text[..2], System.Globalization.NumberStyles.BinaryNumber, null);
				else return T.Parse(token.Value.Text, null);
			}
			catch (Exception)
			{
				throw new InvalidTokenException(token.Value, Token.Type.Integer);
			}
		}
    }
}
