using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public class VariableExpression(string name) : UnresolvedExpression
	{
		public readonly string Name = name;

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var variable = scope.GetVariable(Name);

			return new ResolvedVariableExpression(variable, variable.Type);
		}
	}

	public class ResolvedVariableExpression(Variable variable, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly Variable Variable = variable;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedVariableExpression(Variable, type);

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			Variable.Read(regs);
		}
	}
}
