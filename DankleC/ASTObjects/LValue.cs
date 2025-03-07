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
		public abstract void WriteFrom(IValue val, IRBuilder builder);
		public abstract IValue PostIncrement(IRBuilder builder);
		public abstract IValue PreIncrement(IRBuilder builder);
	}
}
