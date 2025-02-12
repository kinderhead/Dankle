using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class CastExpression(IExpression expr, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly IExpression Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new CastExpression(Expr, type);

        public override IValue Execute()
        {
            throw new NotImplementedException();
        }
    }
}
