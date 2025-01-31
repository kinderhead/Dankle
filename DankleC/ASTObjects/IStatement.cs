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
		public int ID { get; }

		public void BuildIR(IRBuilder builder, IRFunction func, IRScope scope);

		public static readonly Random IDRandomizer = new();
	}

	public class ReturnStatement(IExpression expression) : IStatement
	{
		public int ID { get; } = IStatement.IDRandomizer.Next();
		public readonly IExpression Expression = expression;

		public void BuildIR(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var expr = builder.Cast(Expression.Resolve(builder, func, scope), func.ReturnType);

			if (func.ReturnType.Size <= 2) expr.WriteToRegisters([0], builder);
			else if (func.ReturnType.Size <= 4) expr.WriteToRegisters([0, 1], builder);
			else throw new NotImplementedException();
		}
	}

	public class InitAssignmentStatement(TypeSpecifier type, string name, IExpression expr) : IStatement
	{
		public int ID { get; } = IStatement.IDRandomizer.Next();
		public readonly TypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly IExpression Expression = expr;

		public void BuildIR(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var expr = builder.Cast(Expression.Resolve(builder, func, scope), Type);
			var variable = scope.AllocLocal(Name, Type);
			variable.Write(expr);
		}
	}
}
