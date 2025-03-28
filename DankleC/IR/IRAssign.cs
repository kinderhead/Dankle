using System;
using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using DankleC.ASTObjects;
using DankleC.ASTObjects.Expressions;

namespace DankleC.IR
{
    // Does not return
    public class IRAssign(IPointer dest, ArithmeticOperation op, IValue val, TypeSpecifier type) : IRInsn
    {
        public readonly IPointer Dest = dest;
        public readonly ArithmeticOperation Op = op;
        public readonly IValue Value = val;
        public readonly TypeSpecifier Type = type;

        public override void Compile(CodeGen gen)
        {
            if (Op == ArithmeticOperation.Null) Value.WriteTo(this, Dest);
            else if (Type.IsNumber())
            {
                IValue left = new SimplePointerValue(Dest, Type, Scope);
                IValue right = Value;

                if (Type.Size == 1 && Type.IsSigned())
                {
                    left = left.ToRegisters(this);
                    Add(CGInsn.Build<SignExtend8>(left.MakeArg(), left.MakeArg()));

                    if (right is not IImmediateValue)
                    {
                        right = right.ToRegisters(this);
                        Add(CGInsn.Build<SignExtend8>(right.MakeArg(), right.MakeArg()));
                    }
                }
                else if (Type.Size == 1)
                {
                    left = left.ToRegisters(this);
                    if (right is not IImmediateValue)
                    {
                        right = right.ToRegisters(this);
                    }
                }

                IRMath.Perform(this, left, Op, right, left);

                if (Type.Size == 1) left.WriteTo(this, Dest);
            }
            else throw new InvalidOperationException();
        }
    }
}
