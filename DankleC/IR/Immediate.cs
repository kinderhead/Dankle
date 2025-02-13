using System;
using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using DankleC.ASTObjects;

namespace DankleC.IR
{
	public class Immediate(ushort value, BuiltinType type) : IImmediateValue
    {
        public readonly ushort Value = value;

        public Type CGType => typeof(CGImmediate<ushort>);

        public TypeSpecifier Type => new BuiltinTypeSpecifier(type);

		public ICGArg MakeArg() => new CGImmediate<ushort>(Value);

        public SimpleRegisterValue ToRegisters(IRInsn insn)
        {
            var reg = insn.Alloc();
            WriteTo(insn, [reg]);
            return new([reg], Type);
        }

        public void WriteTo(IRInsn insn, IPointer ptr)
        {
            var reg = insn.OneTimeAlloc();
            if (Type.Size == 1)
            {
                insn.Add(CGInsn.Build<Load8>(new CGRegister(reg), new CGImmediate<byte>((byte)Value)));
                insn.Add(CGInsn.Build<Store8>(ptr.Build<byte>(insn.Scope), new CGRegister(reg)));
            }
            else
            {
                insn.Add(CGInsn.Build<Load>(new CGRegister(reg), MakeArg()));
                insn.Add(CGInsn.Build<Store>(ptr.Build<ushort>(insn.Scope), new CGRegister(reg)));
            }
        }

        public void WriteTo(IRInsn insn, int[] regs)
        {
            if (regs.Length != 1) throw new InvalidOperationException();
            if (Type.Size == 1) insn.Add(CGInsn.Build<Load8>(new CGRegister(regs[0]), new CGImmediate<byte>((byte)Value)));
            else insn.Add(CGInsn.Build<Load>(new CGRegister(regs[0]), MakeArg()));
		}
	}

	public class Immediate32(uint value, BuiltinType type) : IImmediateValue
	{
		public readonly uint Value = value;

		public Type CGType => typeof(CGImmediate<uint>);

		public TypeSpecifier Type => new BuiltinTypeSpecifier(type);

		public ICGArg MakeArg() => new CGImmediate<uint>(Value);
        
        public SimpleRegisterValue ToRegisters(IRInsn insn)
        {
            var regs = insn.Alloc(4);
            WriteTo(insn, regs);
            return new(regs, Type);
        }

		public void WriteTo(IRInsn insn, IPointer ptr)
        {
            var reg = insn.OneTimeAlloc();
            insn.Add(CGInsn.Build<Load>(new CGRegister(reg), new CGImmediate<ushort>((ushort)(Value >>> 16))));
            insn.Add(CGInsn.Build<Store>(ptr.Build<ushort>(insn.Scope), new CGRegister(reg)));
            insn.Add(CGInsn.Build<Load>(new CGRegister(reg), new CGImmediate<ushort>((ushort)(Value & 0xFFFF))));
            insn.Add(CGInsn.Build<Store>(ptr.Get(2).Build<ushort>(insn.Scope), new CGRegister(reg)));
        }

		public void WriteTo(IRInsn insn, int[] regs)
		{
			if (regs.Length != 2) throw new InvalidOperationException();
			insn.Add(CGInsn.Build<Load>(new CGRegister(regs[0]), new CGImmediate<ushort>((ushort)(Value >>> 16))));
			insn.Add(CGInsn.Build<Load>(new CGRegister(regs[1]), new CGImmediate<ushort>((ushort)(Value & 0xFFFF))));
		}
	}
}
