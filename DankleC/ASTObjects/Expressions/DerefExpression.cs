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

		public override ResolvedExpression Resolve(IRBuilder builder) => new ResolvedDerefExpression(Expr.Resolve(builder));
	}

	public class ResolvedDerefExpression(ResolvedExpression expr) : LValue(((PointerTypeSpecifier)expr.Type).Inner)
	{
		public readonly ResolvedExpression Expr = expr;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type)
		{
			throw new NotImplementedException();
		}

		public override IValue Execute(IRBuilder builder)
		{
			if (!Type.IsNumber()) throw new NotImplementedException();

			var ptr = Expr.Execute(builder);
			builder.Add(new IRDynLoadPtr(ptr, Type));

			return ReturnValue();
		}

		public override IValue GetRef(IRBuilder builder)
		{
			return Expr.Execute(builder);
		}

        public override IValue PostIncrement(IRBuilder builder)
        {
            throw new NotImplementedException();
        }

        public override IValue PreIncrement(IRBuilder builder)
        {
            throw new NotImplementedException();
        }

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
			Expr.Walk(cb);
		}

		public override void WriteFrom(IValue val, IRBuilder builder)
		{
			TempStackVariable? save = null;

			if (val is SimpleRegisterValue)
			{
				save = builder.CurrentScope.AllocTemp(Expr.Type);
				save.Store(builder, val);
				val = save;
			}

			var ptr = Expr.Execute(builder);
			builder.Add(new IRDynStorePtr(ptr, val));
			save?.Dispose();
        }
    }
}
