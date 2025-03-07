using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class RefExpression(UnresolvedLValue expr) : UnresolvedExpression
	{
		public readonly UnresolvedLValue Expr = expr;

		public override ResolvedExpression Resolve(IRBuilder builder) => new ResolvedRefExpression(Expr.Resolve<LValue>(builder));
	}

	public class ResolvedRefExpression(LValue expr) : ResolvedExpression(expr.Type.AsPointer())
	{
		public readonly LValue Expr = expr;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type)
		{
			throw new NotImplementedException();
		}

        public override IValue Execute(IRBuilder builder)
		{
			return Expr.GetRef(builder);
        }

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
			Expr.Walk(cb);
		}
	}
}
