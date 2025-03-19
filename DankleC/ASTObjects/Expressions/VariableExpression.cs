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

        public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var variable = builder.CurrentScope.GetVariable(Name);

			return new ResolvedVariableExpression(variable, variable.Type);
		}
	}

	public class ResolvedVariableExpression(Variable variable, TypeSpecifier type) : LValue(type)
	{
		public readonly Variable Variable = variable;

        public override bool IsSimpleExpression => true;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedVariableExpression(Variable, type);

		public override IValue Execute(IRBuilder builder) => Variable;

		public override IValue GetRef(IRBuilder builder)
		{
			if (Variable is PointerVariable v)
			{
				builder.Add(new IRLoadPtrAddress(v.Pointer));
				return new SimpleRegisterValue(IRInsn.FitRetRegs(Type.AsPointer().Size), Type.AsPointer());
			}
			else throw new InvalidOperationException();
        }

		public override IPointer GetPointer(IRBuilder builder) => Variable is PointerVariable v ? v.Pointer : throw new InvalidOperationException();

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
		}

		public override void WriteFrom(IValue val, IRBuilder builder, int offset, int subTypeSize)
		{
			if (Variable is not PointerVariable v) throw new InvalidOperationException();

			builder.Add(new IRStorePtr(v.Pointer.Get(offset, subTypeSize), val));
        }
    }
}
