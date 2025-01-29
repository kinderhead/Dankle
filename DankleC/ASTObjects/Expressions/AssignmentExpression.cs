using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class AssignmentExpression(string name, IExpression expr) : UnresolvedExpression
	{
		public readonly string Name = name;
		public readonly IExpression Expression = expr;

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var variable = scope.GetVariable(Name);
			var expr = builder.Cast(Expression.Resolve(builder, func, scope), variable.Type);
			variable.Write(expr);
			return new ResolvedVariableExpression(variable, variable.Type);
		}
	}
}
