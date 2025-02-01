using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public abstract class Variable(string name, TypeSpecifier type, IRScope scope)
	{
		public readonly string Name = name;
		public readonly TypeSpecifier Type = type;
		public readonly IRScope Scope = scope;

		public abstract void ReadTo(int[] regs);
		public abstract void ReadTo(IPointer ptr);
		public abstract void WriteFrom(ResolvedExpression expr);
	}

	public class RegisterVariable(string name, TypeSpecifier type, int[] reg, IRScope scope) : Variable(name, type, scope)
	{
		public readonly int[] Registers = reg;

		public override void ReadTo(int[] regs)
		{
			if (regs.Length != Registers.Length) throw new InvalidOperationException("Mismatched register count");

			for (int i = 0; i < Registers.Length; i++)
			{
				if (regs[i] < 0) continue;
				Scope.Builder.Add(new MoveReg(regs[i], Registers[i]));
			}
		}

		public override void ReadTo(IPointer ptr)
		{
			Scope.Builder.MovRegsToPtr(Registers, ptr);
		}

		public override void WriteFrom(ResolvedExpression expr)
		{
			if (expr.Type.Size != Type.Size) throw new InvalidOperationException();
			expr.WriteToRegisters(Registers, Scope.Builder);
		}
	}

	public class StackVariable(string name, TypeSpecifier type, StackPointer pointer, IRScope scope) : Variable(name, type, scope)
	{
		public readonly StackPointer Pointer = pointer;

		public override void ReadTo(int[] regs)
		{
			Scope.Builder.MovPtrToRegs(Pointer, regs);
		}

		public override void ReadTo(IPointer ptr)
		{
			Scope.Builder.MovePtrToPtr(Pointer, ptr);
		}

		public override void WriteFrom(ResolvedExpression expr)
		{
			if (expr.Type.Size != Type.Size) throw new InvalidOperationException("Mismatched size");
			expr.WriteToPointer(Pointer, Scope.Builder);
		}
	}
}
