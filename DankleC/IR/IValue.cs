using System;
using Dankle.Components.CodeGen;
using DankleC.ASTObjects;

namespace DankleC.IR
{
    public interface IValue
    {
        public TypeSpecifier Type { get; }
        public Type CGType { get; }

        public ICGArg MakeArg();
        public void WriteTo(IRInsn insn, IPointer ptr);
        public void WriteTo(IRInsn insn, int[] regs);

        public SimpleRegisterValue ToRegisters(IRInsn insn);
    }

    public interface IImmediateValue : IValue
    {

    }

    public interface IRegisterValue : IValue
    {
        public int[] Registers { get; }

        public CGRegister MakeArg(int reg);
    }

    public interface IPointerValue : IValue
    {
        public IPointer Pointer { get; }
    }

    public class SimpleRegisterValue(int[] regs, TypeSpecifier type) : IRegisterValue
    {
        public int[] Registers => regs;

        public TypeSpecifier Type => type;

        public Type CGType => Registers.Length == 1 ? typeof(CGRegister) : typeof(CGDoubleRegister);

        public ICGArg MakeArg()
        {
            if (Registers.Length == 1) return new CGRegister(Registers[0]);
			else if (Registers.Length == 2) return new CGDoubleRegister(Registers[0], Registers[1]);
			throw new NotImplementedException();
        }

        public CGRegister MakeArg(int reg) => new(Registers[reg]);

		public SimpleRegisterValue ToRegisters(IRInsn insn) => this;
        public void WriteTo(IRInsn insn, IPointer ptr) => insn.MoveRegsToPtr(Registers, ptr);
        public void WriteTo(IRInsn insn, int[] regs) => insn.MoveRegsToRegs(Registers, regs);
    }
}
