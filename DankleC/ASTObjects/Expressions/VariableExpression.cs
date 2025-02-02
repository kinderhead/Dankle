using Antlr4.Runtime.Misc;
using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class VariableExpression(string name) : UnresolvedLValue
	{
		public readonly string Name = name;

		public override void MarkReferenceable(IRScope scope)
		{
			scope.RequireStackAlloc(Name);
		}

		public override void PrepScope(IRScope scope) { }

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var variable = scope.GetVariable(Name);

			return new ResolvedVariableExpression(variable, variable.Type);
		}
	}

	public class ResolvedVariableExpression(Variable variable, TypeSpecifier type) : LValue(type)
	{
		public readonly Variable Variable = variable;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedVariableExpression(Variable, type);

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			Variable.ReadTo(regs);
		}

		public override int[] GetOrWriteToRegisters(int[] regs, IRBuilder builder)
		{
			if (Variable is RegisterVariable regvar)
			{
				if (regs.Length == regvar.Registers.Length) return regvar.Registers;
				throw new InvalidOperationException();
			}
			else if (Variable is StackVariable stkvar)
			{
				stkvar.ReadTo(regs);
				return regs;
			}
			throw new NotImplementedException();
		}

		public override void WriteToPointer(IPointer pointer, IRBuilder builder)
		{
			Variable.ReadTo(pointer);
		}

		public override void PrepScope(IRScope scope) { }

		public override IPointer GetRef()
		{
			if (Variable is not StackVariable var) throw new InvalidOperationException();
			return var.Pointer;
		}
	}
}
