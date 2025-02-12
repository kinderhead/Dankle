using System;
using Dankle.Components.CodeGen;
using DankleC.ASTObjects;

namespace DankleC.IR
{
    public interface IValue
    {
        public Type CGType { get; }

        public ICGArg MakeArg();
        public void WriteTo(IRInsn insn, IPointer ptr);
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
