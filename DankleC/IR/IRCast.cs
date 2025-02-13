using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public class IRCast(IValue value, TypeSpecifier type) : IRInsn
	{
		public readonly IValue Value = value;
		public readonly TypeSpecifier Type = type;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Type);

			if (Value is IRegisterValue r)
			{
				if (Type.Size <= Value.Type.Size)
				{
					MoveRegsToRegs(r.Registers[(IRBuilder.NumRegForBytes(Value.Type.Size) - IRBuilder.NumRegForBytes(Type.Size))..], ret.Registers);
				}
			}
			else if (Value is IPointerValue p)
			{

			}
			else throw new NotImplementedException();
		}
	}
}
