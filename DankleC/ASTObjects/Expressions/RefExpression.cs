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

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => new ResolvedRefExpression(Expr.Resolve<LValue>(builder, func, scope));
	}

	public class ResolvedRefExpression(LValue expr) : ResolvedExpression(expr.Type.AsPointer())
	{
		public readonly LValue Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type)
		{
			throw new NotImplementedException();
		}

        public override IValue Execute()
        {
            throw new NotImplementedException();
        }
    }
}
