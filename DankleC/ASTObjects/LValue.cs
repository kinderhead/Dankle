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
			else if (ptr is IPointerValue pval) return pval.Pointer;
			else if (ptr is Immediate32 imm) return new LiteralPointer(imm.Value, Type.Size);
			else throw new InvalidOperationException();
		}
	}
}
