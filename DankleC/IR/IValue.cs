using System;
using System.Numerics;
using Dankle.Components.CodeGen;
using DankleC.ASTObjects;

namespace DankleC.IR
{
    public interface IValue
    {
        public TypeSpecifier Type { get; }
        public Type CGType { get; }

        public ICGArg MakeArg();
        public ICGArg MakeArg(int arg);
        public void WriteTo(IRInsn insn, IPointer ptr);
        public void WriteTo(IRInsn insn, int[] regs);

        public IValue ChangeType(TypeSpecifier type);

        public SimpleRegisterValue ToRegisters(IRInsn insn);
        public IValue ToNotPointer(IRInsn insn);

        public ICGArg AsPointer(IRInsn insn) => AsPointer<uint>(insn);
        public ICGArg AsPointer<T>(IRInsn insn) where T : IBinaryInteger<T>;
    }

    public interface IImmediateValue : IValue
    {
        public IByteLike ToBytes();
    }

    public interface IRegisterValue : IValue
    {
        public int[] Registers { get; }
    }

    public interface IPointerValue : IValue
    {
        public IPointer Pointer { get; }
    }

	public class VoidValue : IValue
	{
		public TypeSpecifier Type => new BuiltinTypeSpecifier(BuiltinType.Void);

		public Type CGType => throw new InvalidOperationException();
        public ICGArg AsPointer<T>(IRInsn insn) where T : IBinaryInteger<T> => throw new InvalidOperationException();
        public IValue ChangeType(TypeSpecifier type) => throw new InvalidOperationException();
        public ICGArg MakeArg() => throw new InvalidOperationException();
        public ICGArg MakeArg(int arg) => throw new InvalidOperationException();
        public IValue ToNotPointer(IRInsn insn) => throw new InvalidOperationException();
        public SimpleRegisterValue ToRegisters(IRInsn insn) => throw new InvalidOperationException();
		public void WriteTo(IRInsn insn, IPointer ptr) => throw new InvalidOperationException();
		public void WriteTo(IRInsn insn, int[] regs) => throw new InvalidOperationException();
	}

	public class SimpleRegisterValue(int[] regs, TypeSpecifier type) : IRegisterValue
    {
        public int[] Registers => regs;

        public TypeSpecifier Type => type;

        public Type CGType => Registers.Length switch
        {
            1 => typeof(CGRegister),
            4 => typeof(CGQuadRegister),
			_ => typeof(CGDoubleRegister),
		};

        public ICGArg AsPointer<T>(IRInsn insn) where T : IBinaryInteger<T>
		{
			if (Type.Size != 4) throw new InvalidOperationException();

			var regs = ToRegisters(insn);
			return CGPointer<T>.Make(regs.Registers[0], regs.Registers[1]);
		}

        public IValue ChangeType(TypeSpecifier type) => new SimpleRegisterValue(Registers, type);
        
        public ICGArg MakeArg()
        {
            if (Registers.Length == 1) return new CGRegister(Registers[0]);
            else if (Registers.Length == 2) return new CGDoubleRegister(Registers[0], Registers[1]);
            else if (Registers.Length == 4) return new CGQuadRegister(Registers[0], Registers[1], Registers[2], Registers[3]);
            throw new NotImplementedException();
        }

        public ICGArg MakeArg(int reg) => new CGRegister(Registers[reg]);
        public IValue ToNotPointer(IRInsn insn) => this;
        public SimpleRegisterValue ToRegisters(IRInsn insn) => this;
        public void WriteTo(IRInsn insn, IPointer ptr) => insn.MoveRegsToPtr(Registers, ptr);
        public void WriteTo(IRInsn insn, int[] regs) => insn.MoveRegsToRegs(Registers, regs);
    }

    public class SimplePointerValue(IPointer ptr, TypeSpecifier type, IRScope scope) : IPointerValue
    {
        public IPointer Pointer => ptr;
        public TypeSpecifier Type => type;

        public Type CGType => Type.Size switch
		{
			1 => typeof(CGPointer<byte>),
			4 => typeof(CGPointer<uint>),
			8 => typeof(CGPointer<ulong>),
			_ => typeof(CGPointer<ushort>)
		};
        
        public ICGArg AsPointer<T>(IRInsn insn) where T : IBinaryInteger<T>
		{
			if (Type.Size != 4) throw new InvalidOperationException();

			var regs = ToRegisters(insn);
			return CGPointer<T>.Make(regs.Registers[0], regs.Registers[1]);
		}

        public IValue ChangeType(TypeSpecifier type) => new SimplePointerValue(Pointer, type, scope);

        public ICGArg MakeArg() => Type.Size switch
        {
            1 => Pointer.Build<byte>(scope),
            4 => Pointer.Build<uint>(scope),
            8 => Pointer.Build<ulong>(scope),
			_ => Pointer.Build<ushort>(scope)
        };

        public ICGArg MakeArg(int arg)
        {
            if (IRBuilder.NumRegForBytes(Type.Size) < arg) throw new InvalidOperationException();
            return Pointer.Get(arg * 2).Build<ushort>(scope);
        }

        public IValue ToNotPointer(IRInsn insn) => ToRegisters(insn);

        public SimpleRegisterValue ToRegisters(IRInsn insn)
        {
            var regs = insn.Alloc(Type.Size);
			WriteTo(insn, regs);
			return new(regs, Type);
        }

        public void WriteTo(IRInsn insn, IPointer ptr)
        {
            insn.MovePtrToPtr(Pointer, ptr);
        }

        public void WriteTo(IRInsn insn, int[] regs)
        {
            insn.MovePtrToRegs(Pointer, regs);
        }
    }
}
