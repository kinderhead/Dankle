using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public abstract class Statement : IASTObject
	{
		public int ID { get; } = IDRandomizer.Next();

#pragma warning disable CS8618
        public IRScope Scope { get; internal set; }
#pragma warning restore CS8618

		public abstract void PrepScope();
        public abstract void BuildIR(IRBuilder builder, IRFunction func);

		public static readonly Random IDRandomizer = new();
	}

	public class ReturnStatement(IExpression expression) : Statement
	{
		public readonly IExpression Expression = expression;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			var expr = Expression.Resolve(builder, func, Scope).Cast(func.ReturnType);

			if (func.ReturnType.Size <= 2) expr.WriteToRegisters([0], builder);
			else if (func.ReturnType.Size <= 4) expr.WriteToRegisters([0, 1], builder);
			else throw new NotImplementedException();
		}

		public override void PrepScope()
		{
			Expression.PrepScope(Scope);
		}
	}

	public class InitAssignmentStatement(TypeSpecifier type, string name, IExpression expr) : Statement
	{
		public readonly TypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly IExpression Expression = expr;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			var expr = Expression.Resolve(builder, func, Scope).Cast(Type);
			var variable = Scope.AllocLocal(Name, Type);
			variable.WriteFrom(expr);
		}

		public override void PrepScope()
		{
			Expression.PrepScope(Scope);
		}
	}

	public class AssignmentStatement(string name, IExpression expr) : Statement
	{
		public readonly string Name = name;
		public readonly IExpression Expression = expr;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			var variable = Scope.GetVariable(Name);
			var expr = Expression.Resolve(builder, func, Scope).Cast(variable.Type);
			variable.WriteFrom(expr);
		}

		public override void PrepScope()
		{
			Expression.PrepScope(Scope);
		}
	}

	public class DeclareStatement(TypeSpecifier type, string name) : Statement
	{
		public readonly TypeSpecifier Type = type;
		public readonly string Name = name;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			var variable = Scope.AllocLocal(Name, Type);
		}

		public override void PrepScope()
		{
			
		}
	}
}
