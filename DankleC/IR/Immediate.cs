using System;
using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using DankleC.ASTObjects;

namespace DankleC.IR
{
    public class Immediate(ushort value) : IImmediateValue
    {
        public readonly ushort Value = value;

        public Type CGType => typeof(CGImmediate<ushort>);

        public ICGArg MakeArg() => new CGImmediate<ushort>(Value);

        public void WriteTo(IRInsn insn, IPointer ptr)
        {
            var reg = insn.OneTimeAlloc();
            insn.Add(CGInsn.Build<Load>(new CGRegister(reg), MakeArg()));
            insn.Add(CGInsn.Build<Store>(ptr.Build<ushort>(insn.Scope), new CGRegister(reg)));
        }
    }
}
