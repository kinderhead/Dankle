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

        public int[] ToRegisters(IRInsn insn);
    }

    public interface IImmediateValue : IValue
    {

    }

    public interface IRegisterValue : IValue
    {

    }

    public interface IPointerValue : IValue
    {

    }

    public class SimpleRegisterValue(int[] regs, TypeSpecifier type) : IRegisterValue
    {
        public readonly int[] Registers = regs;

        public TypeSpecifier Type => type;

        public Type CGType => Registers.Length == 1 ? typeof(CGRegister) : typeof(CGDoubleRegister);

        public ICGArg MakeArg()
        {
            if (Registers.Length == 1) return new CGRegister(Registers[0]);
			else if (Registers.Length == 2) return new CGDoubleRegister(Registers[0], Registers[1]);
			throw new NotImplementedException();
        }

        public int[] ToRegisters(IRInsn insn) => Registers;
        public void WriteTo(IRInsn insn, IPointer ptr) => insn.MoveRegsToPtr(Registers, ptr);
        public void WriteTo(IRInsn insn, int[] regs) => insn.MoveRegsToRegs(Registers, regs);
    }
}
