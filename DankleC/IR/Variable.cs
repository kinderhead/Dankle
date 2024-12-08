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

		public abstract void Read(int[] regs);
		public abstract void Write(ResolvedExpression expr);
	}

	public class RegisterVariable(string name, TypeSpecifier type, int[] reg, IRScope scope) : Variable(name, type, scope)
	{
		public readonly int[] Registers = reg;

		public override void Read(int[] regs)
		{
			if (regs.Length != Registers.Length) throw new InvalidOperationException("Mismatched register count");

			for (int i = 0; i < Registers.Length; i++)
			{
				Scope.Builder.Add(new MoveReg(regs[i], Registers[i]));
			}
		}

		public override void Write(ResolvedExpression expr)
		{
			if (expr.Type.Size != Type.Size) throw new InvalidOperationException();
			expr.WriteToRegisters(Registers, Scope.Builder);
		}
	}
}
