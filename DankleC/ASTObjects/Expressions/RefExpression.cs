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

		public override void PrepScope(IRScope scope)
		{
			Expr.PrepScope(scope);
			Expr.MarkReferenceable(scope);
		}

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => new ResolvedRefExpression(Expr.Resolve<LValue>(builder, func, scope));
	}

	public class ResolvedRefExpression(LValue expr) : ResolvedExpression(expr.Type.AsPointer())
	{
		public readonly LValue Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type)
		{
			throw new NotImplementedException();
		}

		public override void PrepScope(IRScope scope)
		{
			Expr.PrepScope(scope);
		}

		public override void WriteToPointer(IPointer pointer, IRBuilder builder)
		{
			if (pointer.Size < 2) throw new InvalidOperationException();
			builder.Add(new LeaPtr(pointer, Expr.GetRef()));
		}

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			if (regs.Length != 2) throw new InvalidOperationException();
			builder.Add(new LeaReg(regs[0], regs[1], Expr.GetRef()));
		}
	}
}
