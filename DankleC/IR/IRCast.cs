using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
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
				else
				{
					MoveRegsToRegsReversed(r.Registers, ret.Registers[(IRBuilder.NumRegForBytes(Type.Size) - IRBuilder.NumRegForBytes(Value.Type.Size))..]);
				}
			}
			else if (Value is IPointerValue p)
			{
				if (Type.Size <= Value.Type.Size)
				{
					MovePtrToRegs(p.Pointer.Get(Value.Type.Size - Type.Size), ret.Registers);
				}
				else
				{
					MovePtrToRegs(p.Pointer, ret.Registers[(IRBuilder.NumRegForBytes(Type.Size) - IRBuilder.NumRegForBytes(Value.Type.Size))..]);
				}
			}
			else throw new NotImplementedException();

			if (Type.Size > Value.Type.Size)
			{
				if (Value.Type.IsSigned())
				{
					if (Type.Size == 4 && Value.Type.Size == 2) Add(CGInsn.Build<SignExtend>(new CGRegister(ret.Registers[1]), new CGRegister(ret.Registers[0])));
					else if (Type.Size == 4 && Value.Type.Size == 1)
					{
						Add(CGInsn.Build<SignExtend8>(new CGRegister(ret.Registers[1]), new CGRegister(ret.Registers[1])));
						Add(CGInsn.Build<SignExtend>(new CGRegister(ret.Registers[1]), new CGRegister(ret.Registers[0])));
					}
					else if (Type.Size == 2 && Value.Type.Size == 1)
					{
						Add(CGInsn.Build<SignExtend8>(new CGRegister(ret.Registers[0]), new CGRegister(ret.Registers[0])));
					}
					else throw new NotImplementedException();
				}
				else
				{
					foreach (var i in ret.Registers[..(IRBuilder.NumRegForBytes(Type.Size) - IRBuilder.NumRegForBytes(Value.Type.Size))])
					{
						Add(CGInsn.Build<Load>(new CGRegister(i), new CGImmediate<ushort>(0)));
					}
				}
			}
		}
	}
}
