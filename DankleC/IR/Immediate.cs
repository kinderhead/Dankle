using System;
using System.Numerics;
using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using DankleC.ASTObjects;

namespace DankleC.IR
{
	public class Immediate(ushort value, BuiltinType type) : IImmediateValue
    {
        public readonly ushort Value = value;

        public Type CGType => Type.Size == 1 ? typeof(CGImmediate<byte>) : typeof(CGImmediate<ushort>);

        public TypeSpecifier Type => new BuiltinTypeSpecifier(type);

        public ICGArg AsPointer<T>(IRInsn insn) where T : IBinaryInteger<T> => throw new InvalidOperationException();

		public ICGArg MakeArg() => Type.Size == 1 ? new CGImmediate<byte>((byte)Value) : new CGImmediate<ushort>(Value);

        public ICGArg MakeArg(int arg)
        {
            if (arg != 0) throw new InvalidOperationException();
            return new CGImmediate<ushort>(Value);
        }

        public byte[] ToBytes()
        {
            if (Type.Size == 1) return [(byte)Value];
            else return [(byte)(Value >> 8), (byte)(Value & 0xFF)];
        }

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

        public ICGArg AsPointer<T>(IRInsn insn) where T : IBinaryInteger<T> => CGPointer<T>.Make(Value);

        public ICGArg MakeArg() => new CGImmediate<uint>(Value);

        public ICGArg MakeArg(int arg)
        {
            if (arg == 0) return new CGImmediate<ushort>((ushort)(Value >>> 16));
            if (arg == 1) return new CGImmediate<ushort>((ushort)(Value & 0xFFFF));
            throw new InvalidOperationException();
        }

        public byte[] ToBytes() => [(byte)(Value >> 24), (byte)(Value >> 16), (byte)(Value >> 8), (byte)(Value & 0xFF)];

        public SimpleRegisterValue ToRegisters(IRInsn insn)
        {
            var regs = insn.Alloc(4);
            WriteTo(insn, regs);
            return new(regs, Type);
        }

        public void WriteTo(IRInsn insn, IPointer ptr)
        {
            if (ptr.Size != 4) throw new InvalidOperationException();
            insn.Add(CGInsn.Build<Store32>(new CGImmediate<uint>(Value), ptr.Build<uint>(insn.Scope)));
            // var reg = insn.OneTimeAlloc();
            // insn.Add(CGInsn.Build<Load>(new CGRegister(reg), new CGImmediate<ushort>((ushort)(Value >>> 16))));
            // insn.Add(CGInsn.Build<Store>(ptr.Build<ushort>(insn.Scope), new CGRegister(reg)));
            // insn.Add(CGInsn.Build<Load>(new CGRegister(reg), new CGImmediate<ushort>((ushort)(Value & 0xFFFF))));
            // insn.Add(CGInsn.Build<Store>(ptr.Get(2).Build<ushort>(insn.Scope), new CGRegister(reg)));
        }

        public void WriteTo(IRInsn insn, int[] regs)
        {
            if (regs.Length != 2) throw new InvalidOperationException();
            insn.Add(CGInsn.Build<Load32>(new CGImmediate<uint>(Value), new CGDoubleRegister(regs[0], regs[1])));
			// insn.Add(CGInsn.Build<Load>(new CGRegister(regs[0]), new CGImmediate<ushort>((ushort)(Value >>> 16))));
            // insn.Add(CGInsn.Build<Load>(new CGRegister(regs[1]), new CGImmediate<ushort>((ushort)(Value & 0xFFFF))));
        }
	}

	public class Immediate64(ulong value, BuiltinType type) : IImmediateValue
	{
		public readonly ulong Value = value;

		public Type CGType => typeof(CGImmediate<ulong>);

		public TypeSpecifier Type => new BuiltinTypeSpecifier(type);

        public ICGArg AsPointer<T>(IRInsn insn) where T : IBinaryInteger<T> => throw new InvalidOperationException();

		public ICGArg MakeArg() => new CGImmediate<ulong>(Value);

		public ICGArg MakeArg(int arg)
		{
			if (arg == 0) return new CGImmediate<ushort>((ushort)(Value >>> 48));
			if (arg == 1) return new CGImmediate<ushort>((ushort)(Value >>> 32));
			if (arg == 2) return new CGImmediate<ushort>((ushort)(Value >>> 16));
			if (arg == 3) return new CGImmediate<ushort>((ushort)(Value & 0xFFFF));
			throw new InvalidOperationException();
		}

        public byte[] ToBytes() => [(byte)(Value >> 56), (byte)(Value >> 48), (byte)(Value >> 40), (byte)(Value >> 32), (byte)(Value >> 24), (byte)(Value >> 16), (byte)(Value >> 8), (byte)(Value & 0xFF)];

        public SimpleRegisterValue ToRegisters(IRInsn insn)
		{
			var regs = insn.Alloc(4);
			WriteTo(insn, regs);
			return new(regs, Type);
		}

        public void WriteTo(IRInsn insn, IPointer ptr)
        {
            if (ptr.Size != 8) throw new InvalidOperationException();
            insn.Add(CGInsn.Build<Store64>(new CGImmediate<ulong>(Value), ptr.Build<ulong>(insn.Scope)));
			// var reg = insn.OneTimeAlloc();
            // insn.Add(CGInsn.Build<Load>(new CGRegister(reg), MakeArg(0)));
            // insn.Add(CGInsn.Build<Store>(ptr.Build<ushort>(insn.Scope), new CGRegister(reg)));
            // insn.Add(CGInsn.Build<Load>(new CGRegister(reg), MakeArg(1)));
            // insn.Add(CGInsn.Build<Store>(ptr.Get(2).Build<ushort>(insn.Scope), new CGRegister(reg)));
            // insn.Add(CGInsn.Build<Load>(new CGRegister(reg), MakeArg(2)));
            // insn.Add(CGInsn.Build<Store>(ptr.Get(4).Build<ushort>(insn.Scope), new CGRegister(reg)));
            // insn.Add(CGInsn.Build<Load>(new CGRegister(reg), MakeArg(3)));
            // insn.Add(CGInsn.Build<Store>(ptr.Get(6).Build<ushort>(insn.Scope), new CGRegister(reg)));
        }

        public void WriteTo(IRInsn insn, int[] regs)
        {
            if (regs.Length != 4) throw new InvalidOperationException();
            insn.Add(CGInsn.Build<Load64>(new CGImmediate<ulong>(Value), new CGQuadRegister(regs[0], regs[1], regs[2], regs[3])));

			// insn.Add(CGInsn.Build<Load>(new CGRegister(regs[0]), MakeArg(0)));
            // insn.Add(CGInsn.Build<Load>(new CGRegister(regs[1]), MakeArg(1)));
            // insn.Add(CGInsn.Build<Load>(new CGRegister(regs[2]), MakeArg(2)));
            // insn.Add(CGInsn.Build<Load>(new CGRegister(regs[3]), MakeArg(3)));
        }
	}
}
