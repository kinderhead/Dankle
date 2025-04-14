using System;
using DankleC.ASTObjects;
using DankleC.IR;

namespace DankleC
{
    public interface IByteLike
    {
        public string Compile();
    }

    public interface IToBytes : IExpression
    {
        public IByteLike ToBytes(IRBuilder builder);
    }

    public readonly record struct Bytes(byte[] Value) : IByteLike
    {
        public string Compile() => string.Join(' ', Value.Select(e => $"0x{e:X2}"));
    }

    public readonly record struct StringVariable(string Value) : IByteLike
    {
        public string Compile() => $".{Value}";
    }

    public readonly record struct ConstantArray(List<IByteLike> Values) : IByteLike
    {
        public string Compile() => string.Join(' ', Values.Select(e => e.Compile()));
    }
}
