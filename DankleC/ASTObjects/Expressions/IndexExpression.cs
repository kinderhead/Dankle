using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class IndexExpression(IExpression source, IExpression expr) : UnresolvedLValue
	{
		public readonly IExpression Source = source;
		public readonly IExpression Expr = expr;

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			return new DerefExpression(new ArithmeticExpression(Source, ArithmeticOperation.Addition, Expr)).Resolve(builder, func, scope);
		}
	}
}
