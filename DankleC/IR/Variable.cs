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
	public abstract class Variable(string name, TypeSpecifier type, IRScope scope) : IValue
	{
		public readonly string Name = name;
		public TypeSpecifier Type => type;
		public readonly IRScope Scope = scope;

		public abstract Type CGType { get; }

		public abstract ICGArg MakeArg();
        public abstract void Store(IRBuilder builder, IValue value);
		public abstract void WriteTo(IRInsn insn, IPointer ptr);
		public abstract void WriteTo(IRInsn insn, int[] regs);
	}

	public class RegisterVariable(string name, TypeSpecifier type, int[] reg, IRScope scope) : Variable(name, type, scope), IRegisterValue
	{
		public readonly int[] Registers = reg;

		public override Type CGType => Registers.Length == 1 ? typeof(CGRegister) : typeof(CGDoubleRegister);

		public override ICGArg MakeArg()
		{
			if (Registers.Length == 1) return new CGRegister(Registers[0]);
			else if (Registers.Length == 2) return new CGDoubleRegister(Registers[0], Registers[1]);
			throw new NotImplementedException();
		}

        public override void Store(IRBuilder builder, IValue value)
		{
			throw new NotImplementedException();
		}

        public override void WriteTo(IRInsn insn, IPointer ptr)
        {
			insn.MoveRegsToPtr(Registers, ptr);
        }

		public override void WriteTo(IRInsn insn, int[] regs)
		{
			insn.MoveRegsToRegs(Registers, regs);
		}
	}

	public class StackVariable(string name, TypeSpecifier type, IPointer pointer, IRScope scope) : Variable(name, type, scope), IPointerValue
	{
		public readonly IPointer Pointer = pointer;

		public override Type CGType => Type.Size switch
		{
			1 => typeof(CGPointer<byte>),
			4 => typeof(CGPointer<uint>),
			_ => typeof(CGPointer<ushort>)
		};

        public override ICGArg MakeArg() => Type.Size switch
		{
			1 => Pointer.Build<byte>(Scope),
			4 => Pointer.Build<uint>(Scope),
			_ => Pointer.Build<ushort>(Scope)
		};

        public override void Store(IRBuilder builder, IValue value)
        {
			builder.Add(new IRStore(Pointer, value));
        }

        public override void WriteTo(IRInsn insn, IPointer ptr)
        {
			insn.MovePtrToPtr(Pointer, ptr);
        }

		public override void WriteTo(IRInsn insn, int[] regs)
		{
			insn.MovePtrToRegs(Pointer, regs);
		}
	}

	public class TempStackVariable(string name, TypeSpecifier type, IPointer pointer, IRScope scope) : StackVariable(name, type, pointer, scope), IDisposable
	{
		public void Dispose()
		{
			Scope.FreeTemp(this);
			GC.SuppressFinalize(this);
		}
	}
}
