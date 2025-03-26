using System;

namespace DankleC.IR
{
    // Does not return
    public class IRAssign(IPointer dest, IValue val) : IRInsn
    {
        public readonly IPointer Dest = dest;
        public readonly IValue Value = val;

        public override void Compile(CodeGen gen)
        {
            Value.WriteTo(this, Dest);
        }
    }
}
