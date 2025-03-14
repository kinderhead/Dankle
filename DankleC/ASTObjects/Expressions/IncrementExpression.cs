using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class PostIncrementExpression(UnresolvedLValue val) : UnresolvedExpression
    {
        public readonly UnresolvedLValue Value = val;

        public override ResolvedExpression Resolve(IRBuilder builder)
        {
            var val = Value.Resolve<LValue>(builder);
			return new ResolvedPostIncrementExpression(val, val.Type);
        }
    }

    public class PreIncrementExpression(UnresolvedLValue val) : UnresolvedExpression
    {
        public readonly UnresolvedLValue Value = val;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var val = Value.Resolve<LValue>(builder);
			return new ResolvedPreIncrementExpression(val, val.Type);
		}
	}

    public class ResolvedPostIncrementExpression(LValue val, TypeSpecifier type) : ResolvedExpression(type)
    {
        public readonly LValue Value = val;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedPostIncrementExpression(Value, type);

        public override IValue Execute(IRBuilder builder)
        {
			if (!Type.IsNumber()) throw new InvalidOperationException();
            builder.Add(new IRPostIncrement(Value.GetPointer(builder), Type));
            return ReturnValue(builder);
        }

        public override ResolvedExpression Standalone() => new ResolvedPreIncrementExpression(Value, Type);

        public override void Walk(Action<ResolvedExpression> cb)
        {
            cb(this);
            Value.Walk(cb);
        }
    }

    public class ResolvedPreIncrementExpression(LValue val, TypeSpecifier type) : ResolvedExpression(type)
    {
        public readonly LValue Value = val;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedPreIncrementExpression(Value, type);

        public override IValue Execute(IRBuilder builder)
        {
			if (!Type.IsNumber()) throw new InvalidOperationException();
			builder.Add(new IRPreIncrement(Value.GetPointer(builder), Type));
			return ReturnValue(builder);
		}

        public override void Walk(Action<ResolvedExpression> cb)
        {
            cb(this);
            Value.Walk(cb);
        }
    }
    
    public class PostDecrementExpression(UnresolvedLValue val) : UnresolvedExpression
    {
        public readonly UnresolvedLValue Value = val;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var val = Value.Resolve<LValue>(builder);
			return new ResolvedPostDecrementExpression(val, val.Type);
		}
	}

    public class PreDecrementExpression(UnresolvedLValue val) : UnresolvedExpression
    {
        public readonly UnresolvedLValue Value = val;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var val = Value.Resolve<LValue>(builder);
			return new ResolvedPreDecrementExpression(val, val.Type);
		}
	}

	public class ResolvedPostDecrementExpression(LValue val, TypeSpecifier type) : ResolvedExpression(type)
    {
        public readonly LValue Value = val;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedPostDecrementExpression(Value, type);

        public override IValue Execute(IRBuilder builder)
		{
			if (!Type.IsNumber()) throw new InvalidOperationException();
			builder.Add(new IRPostDecrement(Value.GetPointer(builder), Type));
			return ReturnValue(builder);
		}

        public override ResolvedExpression Standalone() => new ResolvedPreDecrementExpression(Value, Type);

		public override void Walk(Action<ResolvedExpression> cb)
		{
            cb(this);
            Value.Walk(cb);
		}
	}

    public class ResolvedPreDecrementExpression(LValue val, TypeSpecifier type) : ResolvedExpression(type)
    {
        public readonly LValue Value = val;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedPreDecrementExpression(Value, type);

        public override IValue Execute(IRBuilder builder)
		{
			if (!Type.IsNumber()) throw new InvalidOperationException();
			builder.Add(new IRPreDecrement(Value.GetPointer(builder), Type));
			return ReturnValue(builder);
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
            cb(this);
			Value.Walk(cb);
		}
	}
}
