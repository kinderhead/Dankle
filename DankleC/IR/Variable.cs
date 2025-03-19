using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
		public abstract ICGArg MakeArg(int arg);
		public abstract void Store(IRBuilder builder, IValue value);
		public abstract void WriteTo(IRInsn insn, IPointer ptr);
		public abstract void WriteTo(IRInsn insn, int[] regs);
		public abstract SimpleRegisterValue ToRegisters(IRInsn insn);

		public virtual ICGArg AsPointer<T>(IRInsn insn) where T : IBinaryInteger<T>
		{
			if (Type.Size != 4) throw new InvalidOperationException();

			var regs = ToRegisters(insn);
			return CGPointer<T>.Make(regs.Registers[0], regs.Registers[1]);
		}
    }

	public class RegisterVariable(string name, TypeSpecifier type, int[] reg, IRScope scope) : Variable(name, type, scope), IRegisterValue
	{
		public int[] Registers => reg;

		public override Type CGType => Registers.Length == 1 ? typeof(CGRegister) : typeof(CGDoubleRegister);

		public override ICGArg MakeArg()
		{
			if (Registers.Length == 1) return new CGRegister(Registers[0]);
			else if (Registers.Length == 2) return new CGDoubleRegister(Registers[0], Registers[1]);
			throw new NotImplementedException();
		}

		public override ICGArg MakeArg(int reg) => new CGRegister(Registers[reg]);

		public override void Store(IRBuilder builder, IValue value)
		{
			throw new NotImplementedException();
		}

		public override SimpleRegisterValue ToRegisters(IRInsn insn) => new(Registers, Type);

		public override void WriteTo(IRInsn insn, IPointer ptr)
		{
			insn.MoveRegsToPtr(Registers, ptr);
		}

		public override void WriteTo(IRInsn insn, int[] regs)
		{
			insn.MoveRegsToRegs(Registers, regs);
		}
	}

	public class PointerVariable(string name, TypeSpecifier type, IPointer pointer, IRScope scope) : Variable(name, type, scope), IPointerValue
	{
		public IPointer Pointer => pointer;

		public override Type CGType => Type.Size switch
		{
			1 => typeof(CGPointer<byte>),
			4 => typeof(CGPointer<uint>),
			8 => typeof(CGPointer<ulong>),
			_ => typeof(CGPointer<ushort>)
		};

		public override ICGArg MakeArg() => Type.Size switch
		{
			1 => Pointer.Build<byte>(Scope),
			4 => Pointer.Build<uint>(Scope),
			8 => Pointer.Build<ulong>(Scope),
			_ => Pointer.Build<ushort>(Scope)
		};

		public override ICGArg MakeArg(int arg)
        {
            if (IRBuilder.NumRegForBytes(Type.Size) < arg) throw new InvalidOperationException();
            return Pointer.Get(arg * 2).Build<ushort>(Scope);
        }

		public override void Store(IRBuilder builder, IValue value)
		{
			builder.Add(new IRStorePtr(Pointer, value));
		}

		public override SimpleRegisterValue ToRegisters(IRInsn insn)
		{
			var regs = insn.Alloc(Type.Size);
			WriteTo(insn, regs);
			return new(regs, Type);
		}

		public override void WriteTo(IRInsn insn, IPointer ptr)
		{
			insn.MovePtrToPtr(Pointer, ptr);
		}

		public override void WriteTo(IRInsn insn, int[] regs)
		{
			insn.MovePtrToRegs(Pointer, regs);
		}

		public PointerVariable Index(int offset)
		{
			if (Type is not ArrayTypeSpecifier arr) throw new NotImplementedException();
			return new(Name, arr.Inner, pointer.Get(offset, arr.Inner.Size), Scope);
		}
    }

	public class TempStackVariable(string name, TypeSpecifier type, IPointer pointer, IRScope scope) : PointerVariable(name, type, pointer, scope), IDisposable
	{
		public void Dispose()
		{
			Scope.FreeTemp(this);
			GC.SuppressFinalize(this);
		}
	}

	public class LabelVariable(string name, TypeSpecifier type, IRScope scope) : Variable(name, type, scope)
	{
		public override Type CGType => typeof(CGLabel<uint>);

		public override ICGArg MakeArg() => new CGLabel<uint>(Name);

		public override ICGArg MakeArg(int arg)
		{
			if (arg == 0) return new CGLabel<ushort>($"{Name}#H");
			if (arg == 1) return new CGLabel<ushort>($"{Name}#L");
			throw new InvalidOperationException();
        }

		public override void Store(IRBuilder builder, IValue value) => throw new InvalidOperationException();

		public override SimpleRegisterValue ToRegisters(IRInsn insn)
		{
			var regs = insn.Alloc(Type.Size);

			WriteTo(insn, regs);

			return new(regs, Type);
		}

		public override void WriteTo(IRInsn insn, IPointer ptr)
		{
			if (ptr.Size != 4) throw new InvalidOperationException();
			insn.Add(CGInsn.Build<Load32>(MakeArg(), ptr.Build<uint>(Scope)));
		}

		public override void WriteTo(IRInsn insn, int[] regs)
		{
			if (regs.Length != 2) throw new InvalidOperationException();
			insn.Add(CGInsn.Build<Load32>(MakeArg(), new CGDoubleRegister(regs[0], regs[1])));
		}

		public override ICGArg AsPointer<T>(IRInsn insn) => MakeArg();
    }
}
