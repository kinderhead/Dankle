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
		public abstract IValue GetRef(IRBuilder builder, IRScope scope);
		public abstract void WriteFrom(ResolvedExpression expr, IRBuilder builder);
	}
}
