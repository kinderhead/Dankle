using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class ShiftExpression(IExpression left, bool rightShift, IExpression right) : UnresolvedExpression
    {
        public readonly IExpression Left = left;
        public readonly bool RightShift = rightShift;
        public readonly IExpression Right = right;
        
        public override ResolvedExpression Resolve(IRBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
