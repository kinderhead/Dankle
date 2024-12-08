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
	public abstract class IRInsn
	{
		public abstract void Compile(CodeGen gen);
	}

	public class ReturnInsn : IRInsn
	{
		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<Return>());
		}
	}

	public class LoadImmToReg(int reg, ushort val) : IRInsn
	{
		public readonly int Register = reg;
		public readonly ushort Val = val;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<Load>(new CodeGenRegister(Register), new CodeGenImmediate<ushort>(Val)));
		}
	}

	public class MoveReg(int dest, int src) : IRInsn
	{
		public readonly int Dest = dest;
		public readonly int Src = src;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<Move>(new CodeGenRegister(Dest), new CodeGenRegister(Src)));
		}
	}

	public class PushReg(int reg) : IRInsn
	{
		public readonly int Register = reg;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<Push>(new CodeGenRegister(Register)));
		}
	}

	public class PopReg(int reg) : IRInsn
	{
		public readonly int Register = reg;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<Pop>(new CodeGenRegister(Register)));
		}
	}

	public class PushRegs(ushort regs) : IRInsn
	{
		public readonly ushort Registers = regs;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<PushRegisters>(new CodeGenImmediate<ushort>(Registers)));
		}
	}

	public class PopRegs(ushort regs) : IRInsn
	{
		public readonly ushort Registers = regs;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<PopRegisters>(new CodeGenImmediate<ushort>(Registers)));
		}
	}

	public class AddRegs(int arg1, int arg2, int dest) : IRInsn
	{
		public readonly int Arg1 = arg1;
		public readonly int Arg2 = arg2;
		public readonly int Dest = dest;

		public override void Compile(CodeGen gen)
		{
			if (Arg1 == -1 || Arg2 == -1 || Dest == -1) return;
			gen.Add(CodeGenInsn.Build<Add>(new CodeGenRegister(Arg1), new CodeGenRegister(Arg2), new CodeGenRegister(Dest)));
		}
	}

	public class AdcRegs(int arg1, int arg2, int dest) : IRInsn
	{
		public readonly int Arg1 = arg1;
		public readonly int Arg2 = arg2;
		public readonly int Dest = dest;

		public override void Compile(CodeGen gen)
		{
			if (Arg1 == -1 || Arg2 == -1 || Dest == -1) return;
			gen.Add(CodeGenInsn.Build<Adc>(new CodeGenRegister(Arg1), new CodeGenRegister(Arg2), new CodeGenRegister(Dest)));
		}
	}

	public class SMulRegs(int arg1, int arg2, int dest) : IRInsn
	{
		public readonly int Arg1 = arg1;
		public readonly int Arg2 = arg2;
		public readonly int Dest = dest;

		public override void Compile(CodeGen gen)
		{
			if (Arg1 == -1 || Arg2 == -1 || Dest == -1) return;
			gen.Add(CodeGenInsn.Build<SignedMul>(new CodeGenRegister(Arg1), new CodeGenRegister(Arg2), new CodeGenRegister(Dest)));
		}
	}

	public class UMulRegs(int arg1, int arg2, int dest) : IRInsn
	{
		public readonly int Arg1 = arg1;
		public readonly int Arg2 = arg2;
		public readonly int Dest = dest;

		public override void Compile(CodeGen gen)
		{
			if (Arg1 == -1 || Arg2 == -1 || Dest == -1) return;
			gen.Add(CodeGenInsn.Build<UnsignedMul>(new CodeGenRegister(Arg1), new CodeGenRegister(Arg2), new CodeGenRegister(Dest)));
		}
	}
}
