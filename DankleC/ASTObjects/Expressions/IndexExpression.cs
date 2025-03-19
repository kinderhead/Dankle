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

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			if (Source is VariableExpression v && Expr is ConstantExpression c)
			{
				var expr = (ResolvedVariableExpression)v.Resolve(builder);
				var type = ((ArrayTypeSpecifier)expr.Type).Inner;
				var val = ((PointerVariable)expr.Variable).Index((dynamic)c.Value * type.Size);
				return new ResolvedVariableExpression(val, type);
			}
			return new DerefExpression(new ArithmeticExpression(Source, ArithmeticOperation.Addition, Expr)).Resolve(builder);
		}
	}
}
