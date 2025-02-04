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

		public override void MarkReferenceable(IRScope scope)
		{
			
		}

		public override void PrepScope(IRScope scope)
		{
			Source.PrepScope(scope);
			Expr.PrepScope(scope);
		}

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var val = Source.Resolve<LValue>(builder, func, scope);
			return new ResolvedIndexExpression(val, Expr.Resolve(builder, func, scope), ((ArrayTypeSpecifier)val.Type).Inner);
		}
	}

	public class ResolvedIndexExpression(LValue source, ResolvedExpression expr, TypeSpecifier type) : LValue(type)
	{
		public readonly LValue Source = source;
		public readonly ResolvedExpression Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedIndexExpression(Source, Expr, type);

		public override IPointer GetRef(IRBuilder builder, out IRScope.TempRegHolder? regs, int[] regsInUse)
		{
			var expr = new ArithmeticExpression(Expr, ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), Type.Size)).Resolve(builder, builder.CurrentFunction, builder.CurrentScope).Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt));

			if (expr is ConstantExpression c)
			{
				var offset = (int)c.Value;
				if (offset > short.MaxValue || offset < short.MinValue) throw new NotImplementedException();
				return Source.GetRef(builder, out regs, regsInUse).Get(offset, Type.Size);
			}

			regs = builder.CurrentScope.AllocTempRegs(4, regsInUse);
			builder.Add(new LeaReg(regs.Registers[0], regs.Registers[1], Source.GetRef(builder, out var tmp, [.. regsInUse, .. regs.Registers])));
			if (tmp is not null) throw new NotImplementedException();
			
			var regs2 = builder.CurrentScope.AllocTempRegs(4, [.. regsInUse, .. regs.Registers]);
			var actualRegs2 = expr.GetOrWriteToRegisters(regs2.Registers, builder);
			builder.Add(new AddRegs(regs.Registers[0], actualRegs2[0], regs.Registers[0]));
			builder.Add(new AdcRegs(regs.Registers[1], actualRegs2[1], regs.Registers[1]));
			regs2.Dispose(regs2.Registers[0] == actualRegs2[0]);
			return new RegisterPointer(regs.Registers[0], regs.Registers[1], 0, Type.Size);
		}

		public override void PrepScope(IRScope scope)
		{
			Source.PrepScope(scope);
			Expr.PrepScope(scope);
		}

		public override void WriteFrom(ResolvedExpression expr, IRBuilder builder)
		{
			var ptr = GetRef(builder, out var regs, []);
			expr.Cast(Type).WriteToPointer(ptr, builder, regs?.Registers ?? []);
			regs?.Dispose();
		}

		public override void WriteToPointer(IPointer pointer, IRBuilder builder, int[] usedRegs)
		{
			throw new NotImplementedException();
		}

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			if (regs.Length != IRBuilder.NumRegForBytes(Type.Size)) throw new InvalidOperationException();
			var ptr = GetRef(builder, out var tmp, regs);
			builder.MovePtrToRegs(ptr, regs);
			tmp?.Dispose();
		}
	}
}
