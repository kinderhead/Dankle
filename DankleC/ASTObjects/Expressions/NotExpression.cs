using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class NotExpression(IExpression expr) : UnresolvedExpression
    {
        public readonly IExpression Expression = expr;

        public override ResolvedExpression Resolve(IRBuilder builder) => new EqualityExpression(Expression, EqualityOperation.Equals, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), 0)).Resolve(builder);
    }

    public class BitwiseNotExpression(IExpression expr) : UnresolvedExpression
    {
        public readonly IExpression Expression = expr;

        public override ResolvedExpression Resolve(IRBuilder builder)
        {
            var expr = Expression.Resolve(builder);
            var type = expr.Type;

            if (type.Size == 1 && type.IsSigned()) type = new BuiltinTypeSpecifier(BuiltinType.SignedShort);
            else if (type.Size == 1 && !type.IsSigned()) type = new BuiltinTypeSpecifier(BuiltinType.UnsignedShort);

            return new ResolvedBitwiseNotExpression(expr, type);
        }
    }

    public class ResolvedBitwiseNotExpression(ResolvedExpression expr, TypeSpecifier type) : ResolvedExpression(type)
    {
        public readonly ResolvedExpression Expression = expr;
        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedBitwiseNotExpression(Expression, type);

        public override IValue Execute(IRBuilder builder)
        {
            throw new NotImplementedException();
        }

        public override void Walk(Action<ResolvedExpression> cb)
        {
            cb(this);
            Expression.Walk(cb);
        }
    }
}
