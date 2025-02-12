using System;

namespace DankleC.IR
{
    public class IRAdd(IValue left, IValue right) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;

        public override void Compile(CodeGen gen)
        {
            if (Left.Type != Right.Type || !Left.Type.IsNumber() || !Right.Type.IsNumber()) throw new NotImplementedException();

            if (Left.Type.Size <= 2)
            {
                
            }
        }
    }
}
