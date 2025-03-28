using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class CommaExpression(IExpression first, IExpression second) : UnresolvedExpression
    {
        public readonly IExpression First = first;
        public readonly IExpression Second = second;

        public override ResolvedExpression Resolve(IRBuilder builder)
        {
            var first = First.Resolve(builder);
            var second = Second.Resolve(builder);
            return new ResolvedCommaExpression(first, second, first.Type);
        }
    }

    public class ResolvedCommaExpression(ResolvedExpression first, ResolvedExpression second, TypeSpecifier type) : ResolvedExpression(type)
    {
        public readonly ResolvedExpression First = first;
        public readonly ResolvedExpression Second = second;
        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedCommaExpression(First, Second, type);

        public override IValue Execute(IRBuilder builder)
        {
            First.Standalone().Execute(builder);
            return Second.Cast(Type).Execute(builder);
        }

        public override void Walk(Action<ResolvedExpression> cb)
        {
            cb(this);
            First.Walk(cb);
            Second.Walk(cb);
        }
    }
}
