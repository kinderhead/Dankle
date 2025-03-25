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
            if (Function.Type.Size != 4) throw new InvalidOperationException();
            Add(CGInsn.Build<Call>(Function.MakeArg()));
        }
    }
}
