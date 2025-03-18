using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class NotExpression(IExpression expr) : UnresolvedExpression
    {
        public readonly IExpression Expression = expr;

        public override ResolvedExpression Resolve(IRBuilder builder) => new EqualityExpression(Expression, EqualityOperation.Equals, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), 0)).Resolve(builder);
    }
}
