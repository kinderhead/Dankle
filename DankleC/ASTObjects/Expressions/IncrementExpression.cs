using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class PostIncrementExpression(UnresolvedLValue val) : UnresolvedExpression
    {
        public readonly UnresolvedLValue Value = val;

        public override ResolvedExpression Resolve(IRBuilder builder) => new ResolvedPostIncrementExpression(Value.Resolve<LValue>(builder));
    }

    public class PreIncrementExpression(UnresolvedLValue val) : UnresolvedExpression
    {
        public readonly UnresolvedLValue Value = val;

        public override ResolvedExpression Resolve(IRBuilder builder) => new ResolvedPreIncrementExpression(Value.Resolve<LValue>(builder));
    }

	public class ResolvedPostIncrementExpression(LValue val) : ResolvedExpression(val.Type)
    {
        public readonly LValue Value = val;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedPostIncrementExpression(Value);

        public override IValue Execute(IRBuilder builder)
		{
            return Value.PostIncrement(builder);
        }

        public override ResolvedExpression Standalone() => new ResolvedPreIncrementExpression(Value);

		public override void Walk(Action<ResolvedExpression> cb)
		{
            cb(this);
            Value.Walk(cb);
		}
	}

    public class ResolvedPreIncrementExpression(LValue val) : ResolvedExpression(val.Type)
    {
        public readonly LValue Value = val;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedPostIncrementExpression(Value);

        public override IValue Execute(IRBuilder builder)
		{
            return Value.PreIncrement(builder);
        }

		public override void Walk(Action<ResolvedExpression> cb)
		{
            cb(this);
			Value.Walk(cb);
		}
	}
}
