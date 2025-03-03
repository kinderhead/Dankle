﻿using DankleC.IR;
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

        public abstract void BuildIR(IRBuilder builder, IRFunction func);

		public static readonly Random IDRandomizer = new();
	}

	public class ReturnStatement(IExpression? expression) : Statement
	{
		public readonly IExpression? Expression = expression;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			var expr = Expression?.Resolve(builder, func, Scope);
			var value = expr?.Cast(func.ReturnType).Execute(builder, Scope);

			if (value is not null) builder.Add(new IRSetReturn(value));
			else if (func.ReturnType != new BuiltinTypeSpecifier(BuiltinType.Void)) throw new InvalidOperationException($"Function \"{func.Name}\" must return a value");
			builder.Add(new EndFrame());
			builder.Add(new IRReturnFunc());
		}
	}

	public class InitAssignmentStatement(TypeSpecifier type, string name, IExpression expr) : Statement
	{
		public readonly TypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly IExpression Expression = expr;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			var value = Expression.Resolve(builder, func, Scope).Cast(Type).Execute(builder, Scope);
			var variable = Scope.AllocLocal(Name, Type);
			variable.Store(builder, value);
		}
	}

	public class AssignmentStatement(UnresolvedLValue dest, IExpression expr) : Statement
	{
		public readonly UnresolvedLValue Dest = dest;
		public readonly IExpression Expression = expr;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			var variable = Dest.Resolve<LValue>(builder, func, Scope);
			var expr = Expression.Resolve(builder, func, Scope).Cast(variable.Type);
			variable.WriteFrom(expr.Execute(builder, Scope), builder);
		}
	}

	public class DeclareStatement(TypeSpecifier type, string name) : Statement
	{
		public readonly TypeSpecifier Type = type;
		public readonly string Name = name;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			Scope.AllocLocal(Name, Type);
		}
	}
}
