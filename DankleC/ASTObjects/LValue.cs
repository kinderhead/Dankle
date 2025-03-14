using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public abstract class UnresolvedLValue : UnresolvedExpression
	{
		
	}

	public abstract class LValue(TypeSpecifier type) : ResolvedExpression(type)
	{
		public abstract IValue GetRef(IRBuilder builder);

		public void WriteFrom(IValue val, IRBuilder builder) => WriteFrom(val, builder, 0, Type.Size);
		public abstract void WriteFrom(IValue val, IRBuilder builder, int offset, int subTypeSize);

		// Only valid for one IR instruction
		public virtual IPointer GetPointer(IRBuilder builder)
		{
			var ptr = GetRef(builder);
			if (ptr is SimpleRegisterValue val) return new RegisterPointer(val.Registers[0], val.Registers[1], 0, Type.Size);
			else throw new InvalidOperationException();
		}
	}
}
