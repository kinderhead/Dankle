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

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var expr = Expr.Resolve(builder);
			return new ResolvedDerefExpression(expr, ((PointerTypeSpecifier)expr.Type).Inner);
		}
	}

	public class ResolvedDerefExpression(ResolvedExpression expr, TypeSpecifier type) : LValue(type)
	{
		public readonly ResolvedExpression Expr = expr;

		public override bool IsSimpleExpression => false;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedDerefExpression(Expr, type);

		public override IValue Execute(IRBuilder builder)
		{
			if (!Type.IsNumber()) throw new NotImplementedException();

			var ptr = Expr.Execute(builder);
			builder.Add(new IRDynLoadPtr(ptr, Type));

			return ReturnValue(builder);
		}

		public override IValue GetRef(IRBuilder builder)
		{
			return Expr.Execute(builder);
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
			Expr.Walk(cb);
		}

		public override void WriteFrom(IValue val, IRBuilder builder, int offset, int subTypeSize)
		{
			TempStackVariable? save = null;

			if (val is SimpleRegisterValue && !Expr.IsSimpleExpression)
			{
				save = builder.CurrentScope.AllocTemp(val.Type);
				save.Store(builder, val);
				val = save;
			}

			var ptr = Expr.Execute(builder);
			builder.Add(new IRDynStorePtr(ptr, val, offset, subTypeSize));
			save?.Dispose();
		}

		public override IPointer GetPointer(IRBuilder builder)
		{
			var ptr = GetRef(builder);
			if (ptr is SimpleRegisterValue val) return new RegisterPointer(val.Registers[0], val.Registers[1], 0, Type.Size);
			else if (ptr is IPointerValue)
			{
				builder.Add(new IRSetReturn(ptr));
				var ret = IRInsn.FitRetRegs(4);
				return new RegisterPointer(ret[0], ret[1], 0, Type.Size);
			}
			else if (ptr is Immediate32 imm) return new LiteralPointer(imm.Value, Type.Size);
			else throw new InvalidOperationException();
        }
    }
}
