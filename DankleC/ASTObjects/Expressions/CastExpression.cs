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

		public override void WriteToPointer(IPointer pointer, IRBuilder builder)
		{
			var expr = Expr.Resolve(builder, builder.CurrentFunction, builder.CurrentScope);

			if (Type.Size <= expr.Type.Size)
			{
				expr.ChangeType(Type).WriteToPointer(pointer, builder);
			}
			else throw new NotImplementedException();
		}

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			var expr = Expr.Resolve(builder, builder.CurrentFunction, builder.CurrentScope);

			if (Type.Size <= expr.Type.Size)
			{
				expr.WriteToRegisters([.. Enumerable.Repeat(-1, IRBuilder.NumRegForBytes(expr.Type.Size) - IRBuilder.NumRegForBytes(Type.Size)), .. regs], builder);
			}
			else throw new NotImplementedException();
		}
	}
}
