using Dankle;
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
			return tok.Symbol switch
			{
				Token.Type.Integer or Token.Type.Text or Token.Type.Minus => (0b0000, Utils.ToBytes(Parser.ParseNum<ushort>(tok))),
				Token.Type.OSquareBracket => Parser.ParsePointer(tok),
				_ => throw new InvalidTokenException(tok),
			};
		}
	}

	public class Any8NumParser(Parser parser) : ArgumentParser(parser)
	{
		public override (byte type, byte[] data) Parse()
		{
			var tok = Parser.Tokens.Dequeue();
			return tok.Symbol switch
			{
				Token.Type.Integer or Token.Type.Text => (0b0000, [Parser.ParseNum<byte>(tok)]),
				Token.Type.OSquareBracket => Parser.ParsePointer(tok),
				_ => throw new InvalidTokenException(tok),
			};
		}
	}

	public class Any16Parser(Parser parser) : ArgumentParser(parser)
	{
		public override (byte type, byte[] data) Parse()
		{
			var tok = Parser.Tokens.Dequeue();
			return tok.Symbol switch
			{
				Token.Type.Register => (0b0001, [Parser.ParseRegister(tok)]),
				Token.Type.Integer or Token.Type.Text => (0b0000, Utils.ToBytes(Parser.ParseNum<ushort>(tok))),
				Token.Type.OSquareBracket => Parser.ParsePointer(tok),
				_ => throw new InvalidTokenException(tok),
			};
		}
	}

	public class Any32Parser(Parser parser) : ArgumentParser(parser)
	{
		public override (byte type, byte[] data) Parse()
		{
			var tok = Parser.Tokens.Dequeue();
			return tok.Symbol switch
			{
				Token.Type.OParen => (0b0001, [Parser.ParseStandaloneDoubleRegister(tok)]),
				Token.Type.Integer or Token.Type.Text => (0b0000, Utils.ToBytes(Parser.ParseNum<uint>(tok))),
				Token.Type.OSquareBracket => Parser.ParsePointer(tok),
				_ => throw new InvalidTokenException(tok),
			};
		}
	}

	public class PointerParser(Parser parser) : ArgumentParser(parser)
	{
		public override (byte type, byte[] data) Parse() => Parser.ParsePointer();
	}
}
