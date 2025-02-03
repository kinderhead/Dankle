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

        public override void MarkReferenceable(IRScope scope)
        {
			throw new InvalidOperationException();
        }

        public override void PrepScope(IRScope scope)
		{
			Expr.PrepScope(scope);
		}

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => new ResolvedDerefExpression(Expr.Resolve(builder, func, scope));
	}

	public class ResolvedDerefExpression(ResolvedExpression expr) : LValue(((PointerTypeSpecifier)expr.Type).Inner)
	{
		public readonly ResolvedExpression Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type)
		{
			throw new NotImplementedException();
		}

        public override IPointer GetRef()
        {
            throw new InvalidOperationException();
        }

        public override void PrepScope(IRScope scope)
		{
			Expr.PrepScope(scope);
		}

        public override void WriteFrom(ResolvedExpression expr, IRBuilder builder)
        {
            var tmp = builder.CurrentScope.AllocTempRegs(4);
			var regs = Expr.GetOrWriteToRegisters(tmp.Registers, builder);
			expr.Cast(Type).WriteToPointer(new RegisterPointer(regs[0], regs[1], 0, Type.Size), builder);
			tmp.Dispose(regs[0] == tmp.Registers[0]);
        }

        public override void WriteToPointer(IPointer pointer, IRBuilder builder)
		{
			throw new NotImplementedException();
		}

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			if (regs.Length != IRBuilder.NumRegForBytes(Type.Size)) throw new InvalidOperationException();

			var ptrRegs = new int[2];
			ptrRegs[0] = regs[0];

			IRScope.TempRegHolder? tmp = null;
			if (regs.Length == 1)
			{
				tmp = builder.CurrentScope.AllocTempRegs(2);
				ptrRegs[1] = tmp.Registers[0];
			}
			else ptrRegs[1] = regs[1];

			ptrRegs = Expr.GetOrWriteToRegisters(ptrRegs, builder);
			builder.MovePtrToRegs(new RegisterPointer(ptrRegs[0], ptrRegs[1], 0, Type.Size), regs);
			tmp?.Dispose(ptrRegs[1] == tmp.Registers[0]);
		}
	}
}
