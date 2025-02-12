using System;
using Dankle.Components.CodeGen;
using DankleC.ASTObjects;

namespace DankleC.IR
{
    public interface IValue
    {
        public TypeSpecifier Type { get; }
        public Type CGType { get; }

        public ICGArg MakeArg();
        public void WriteTo(IRInsn insn, IPointer ptr);
        public void WriteTo(IRInsn insn, int[] regs);
	}

    public interface IImmediateValue : IValue
    {

    }

    public interface IRegisterValue : IValue
    {

    }

    public interface IPointerValue : IValue
    {

    }
}
