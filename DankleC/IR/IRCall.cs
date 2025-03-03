using System;
using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;

namespace DankleC.IR
{
    public class IRCall(IValue func) : IRInsn
    {
        public readonly IValue Function = func;

        public override void Compile(CodeGen gen)
        {
            Add(CGInsn.Build<Call>(Function.AsPointer(this)));
        }
    }
}
