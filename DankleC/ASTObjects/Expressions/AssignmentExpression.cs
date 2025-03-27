using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
	public class AssignmentExpression(UnresolvedLValue dest, ArithmeticOperation op, IExpression expr) : UnresolvedExpression
	{
		public readonly UnresolvedLValue Dest = dest;
		public readonly ArithmeticOperation Op = op;
		public readonly IExpression Expression = expr;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var dest = Dest.Resolve<LValue>(builder);
			var expr = Expression.Resolve(builder);
			return new ResolvedAssignmentExpression(dest, Op, expr, dest.Type);
		}
	}

	public class ResolvedAssignmentExpression(LValue dest, ArithmeticOperation op, ResolvedExpression expr, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly LValue Dest = dest;
		public readonly ArithmeticOperation Op = op;
		public readonly ResolvedExpression Expression = expr;

        public override bool IsSimpleExpression => false;
		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedAssignmentExpression(Dest, Op, Expression, type);

		public override IValue Execute(IRBuilder builder)
		{
			var original = Expression.Cast(Type).Execute(builder);
			IValue val = original;

			if (!Expression.IsSimpleExpression && !Dest.IsSimpleExpression)
			{
				val = builder.CurrentScope.AllocTemp(Type);
				((TempStackVariable)val).Store(builder, original);
			}

			var ptr = Dest.GetPointer(builder);

			builder.Add(new IRAssign(ptr, Op, val));

			if (val is TempStackVariable t) t.Dispose();
			return new SimplePointerValue(ptr, Type, builder.CurrentScope);
        }

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
			Dest.Walk(cb);
			Expression.Walk(cb);
        }
    }
}
