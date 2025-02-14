using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class DerefExpression(IExpression expr) : UnresolvedLValue
	{
		public readonly IExpression Expr = expr;

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => new ResolvedDerefExpression(Expr.Resolve(builder, func, scope));
	}

	public class ResolvedDerefExpression(ResolvedExpression expr) : LValue(((PointerTypeSpecifier)expr.Type).Inner)
	{
		public readonly ResolvedExpression Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type)
		{
			throw new NotImplementedException();
		}

        public override IValue Execute(IRBuilder builder, IRScope scope)
		{
			if (!Type.IsNumber()) throw new NotImplementedException();


        }

		public override IValue GetRef(IRBuilder builder, IRScope scope)
		{
			throw new NotImplementedException();
		}

		public override void WriteFrom(ResolvedExpression expr, IRBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
