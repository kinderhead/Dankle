using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class IndexExpression(UnresolvedLValue source, IExpression expr) : UnresolvedExpression
	{
		public readonly UnresolvedLValue Source = source;
		public readonly IExpression Expr = expr;

		public override void PrepScope(IRScope scope)
		{
			Source.PrepScope(scope);
			Expr.PrepScope(scope);
		}

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			throw new NotImplementedException();
		}
	}

	//public class ResolvedIndexExpression(LValue source, ResolvedExpression expr) : ResolvedExpression
}
