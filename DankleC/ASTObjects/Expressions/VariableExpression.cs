﻿using Antlr4.Runtime.Misc;
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

        public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var variable = scope.GetVariable(Name);

			return new ResolvedVariableExpression(variable, variable.Type);
		}
	}

	public class ResolvedVariableExpression(Variable variable, TypeSpecifier type) : LValue(type)
	{
		public readonly Variable Variable = variable;

        public override bool IsSimpleExpression => true;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedVariableExpression(Variable, type);

		public override IValue Execute(IRBuilder builder, IRScope scope) => Variable;

		public override IValue GetRef(IRBuilder builder, IRScope scope)
		{
			if (Variable is StackVariable v)
			{
				builder.Add(new IRLoadPtrAddress(v.Pointer));
				return new SimpleRegisterValue(IRInsn.FitRetRegs(Type.AsPointer().Size), Type.AsPointer());
			}
			else throw new InvalidOperationException();
        }

		public override IValue PostIncrement(IRBuilder builder)
		{
			builder.Add(new IRPostIncrement(Variable));
			return ReturnValue();
        }

        public override IValue PreIncrement(IRBuilder builder)
        {
            builder.Add(new IRPreIncrement(Variable));
			return ReturnValue();
        }

        public override void WriteFrom(IValue val, IRBuilder builder)
        {
			Variable.Store(builder, val);
        }
    }
}
