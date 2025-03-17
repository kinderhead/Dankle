using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class NegationExpression(IExpression expr) : UnresolvedExpression
    {
        public readonly IExpression Expression = expr;

        public override ResolvedExpression Resolve(IRBuilder builder)
        {
            if (Expression is ConstantExpression c)
            {
                return new ConstantExpression(c.Type, -(Int128)(dynamic)c.Value);
            }

            var expr = Expression.Resolve(builder);
            return new ResovledNegationExpression(expr, expr.Type);
        }
    }

    public class ResovledNegationExpression(ResolvedExpression expr, TypeSpecifier type) : ResolvedExpression(type)
    {
        public readonly ResolvedExpression Expression = expr;
        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResovledNegationExpression(Expression, type);

        public override IValue Execute(IRBuilder builder)
        {
            if (!Type.IsNumber()) throw new InvalidOperationException();
            builder.Add(new IRNegate(Expression.Execute(builder)));
            return ReturnValue(builder);
        }

        public override void Walk(Action<ResolvedExpression> cb)
        {
            cb(this);
            Expression.Walk(cb);
        }
    }
}
