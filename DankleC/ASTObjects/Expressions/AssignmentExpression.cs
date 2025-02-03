using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
	public class AssignmentExpression(string name, IExpression expr) : UnresolvedExpression
	{
		public readonly string Name = name;
		public readonly IExpression Expression = expr;

		public override void PrepScope(IRScope scope)
		{
			Expression.PrepScope(scope);
		}

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			throw new NotImplementedException();
			// var variable = scope.GetVariable(Name);
			// var expr = Expression.Resolve(builder, func, scope).Cast(variable.Type);
			// variable.WriteFrom(expr);
			// return new ResolvedVariableExpression(variable, variable.Type);
		}
	}
}
