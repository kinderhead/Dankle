using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class CastExpression(ResolvedExpression expr, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly ResolvedExpression Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new CastExpression(Expr, type);

        public override IValue Execute(IRBuilder builder, IRScope scope)
		{
			builder.Add(new IRCast(Expr.Execute(builder, scope), Type));
			return ReturnValue();
        }
    }
}
