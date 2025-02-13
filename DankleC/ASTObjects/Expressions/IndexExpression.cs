using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class IndexExpression(UnresolvedLValue source, IExpression expr) : UnresolvedLValue
	{
		public readonly UnresolvedLValue Source = source;
		public readonly IExpression Expr = expr;

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			throw new NotImplementedException();
		}
	}

	public class ResolvedIndexExpression(LValue source, ResolvedExpression expr, TypeSpecifier type) : LValue(type)
	{
		public readonly LValue Source = source;
		public readonly ResolvedExpression Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedIndexExpression(Source, Expr, type);

        public override IValue Execute(IRBuilder builder, IRScope scope)
		{
            throw new NotImplementedException();
        }

        public override void WriteFrom(ResolvedExpression expr, IRBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
