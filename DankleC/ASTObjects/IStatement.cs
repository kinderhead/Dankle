using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public interface IStatement : IASTObject
	{
		public void BuildIR(IRBuilder builder, IRFunction func, IRScope scope);
	}

	public class ReturnStatement(IExpression expression) : IStatement
	{
		public readonly IExpression Expression = expression;

		public void BuildIR(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var expr = builder.Cast(Expression.Resolve(builder, func, scope), func.ReturnType);

			if (func.ReturnType.Size == 4) expr.WriteToRegisters([0, 1], builder);
			else throw new NotImplementedException();
		}
	}
}
