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

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => new ResolvedDerefExpression(Expr.Resolve(builder, func, scope));
	}

	public class ResolvedDerefExpression(ResolvedExpression expr) : LValue(((PointerTypeSpecifier)expr.Type).Inner)
	{
		public readonly ResolvedExpression Expr = expr;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type)
		{
			throw new NotImplementedException();
		}

		public override IValue Execute(IRBuilder builder, IRScope scope)
		{
			if (!Type.IsNumber()) throw new NotImplementedException();

			var ptr = Expr.Execute(builder, scope);
			builder.Add(new IRDynLoadPtr(ptr, Type));

			return ReturnValue();
		}

		public override IValue GetRef(IRBuilder builder, IRScope scope)
		{
			return Expr.Execute(builder, scope);
		}

		public override void WriteFrom(ResolvedExpression expr, IRBuilder builder)
		{
			var val = expr.Execute(builder, builder.CurrentScope);
			TempStackVariable? save = null;

			if (val is SimpleRegisterValue)
			{
				save = builder.CurrentScope.AllocTemp(expr.Type);
				save.Store(builder, val);
				val = save;
			}

			var ptr = Expr.Execute(builder, builder.CurrentScope);
			builder.Add(new IRDynStorePtr(ptr, val));
			save?.Dispose();
        }
    }
}
