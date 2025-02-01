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
#pragma warning disable CS8618
		public IRScope Scope;
#pragma warning restore CS8618

		public abstract void Compile(CodeGen gen);
	}

	public class ReturnInsn : IRInsn
	{
		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Return>());
		}
	}

	public class LoadImmToReg(int reg, ushort val) : IRInsn
	{
		public readonly int Register = reg;
		public readonly ushort Val = val;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Load>(new CGRegister(Register), new CGImmediate<ushort>(Val)));
		}
	}

	public class LoadPtrToReg(int reg, IPointer pointer) : IRInsn
	{
		public readonly int Register = reg;
		public readonly IPointer Pointer = pointer;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Load>(new CGRegister(Register), Pointer.Build<ushort>(Scope)));
		}
	}

	public class LoadPtrToReg8(int reg, IPointer pointer) : IRInsn
	{
		public readonly int Register = reg;
		public readonly IPointer Pointer = pointer;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Load8>(new CGRegister(Register), Pointer.Build<byte>(Scope)));
		}
	}

	public class LoadRegToPtr(IPointer pointer, int reg) : IRInsn
	{
		public readonly int Register = reg;
		public readonly IPointer Pointer = pointer;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Store>(Pointer.Build<ushort>(Scope), new CGRegister(Register)));
		}
	}

	public class LoadRegToPtr8(IPointer pointer, int reg) : IRInsn
	{
		public readonly int Register = reg;
		public readonly IPointer Pointer = pointer;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Store8>(Pointer.Build<byte>(Scope), new CGRegister(Register)));
		}
	}

	public class MoveReg(int dest, int src) : IRInsn
	{
		public readonly int Dest = dest;
		public readonly int Src = src;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Move>(new CGRegister(Dest), new CGRegister(Src)));
		}
	}

	public class PushReg(int reg) : IRInsn
	{
		public readonly int Register = reg;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Push>(new CGRegister(Register)));
		}
	}

	public class PopReg(int reg) : IRInsn
	{
		public readonly int Register = reg;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<Pop>(new CGRegister(Register)));
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
			gen.Add(CGInsn.Build<Add>(new CGRegister(Arg1), new CGRegister(Arg2), new CGRegister(Dest)));
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
			gen.Add(CGInsn.Build<Adc>(new CGRegister(Arg1), new CGRegister(Arg2), new CGRegister(Dest)));
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
			gen.Add(CGInsn.Build<SignedMul>(new CGRegister(Arg1), new CGRegister(Arg2), new CGRegister(Dest)));
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
			gen.Add(CGInsn.Build<UnsignedMul>(new CGRegister(Arg1), new CGRegister(Arg2), new CGRegister(Dest)));
		}
	}

	public class InitFrame() : IRInsn
	{
		public override void Compile(CodeGen gen)
		{
			ushort regs = 0;
			foreach (var i in Scope.PreservedRegs)
			{
				regs |= (ushort)(1 << 15 - i);
			}

			if (regs != 0) gen.Add(CGInsn.Build<PushRegisters>(new CGImmediate<ushort>(regs)));
			if (Scope.StackUsed != 0) gen.Add(CGInsn.Build<ModifyStack>(new CGImmediate<ushort>((ushort)-Scope.StackUsed)));
		}
	}

	public class EndFrame() : IRInsn
	{
		public override void Compile(CodeGen gen)
		{
			ushort regs = 0;
			foreach (var i in Scope.PreservedRegs)
			{
				regs |= (ushort)(1 << 15 - i);
			}

			if (Scope.StackUsed != 0) gen.Add(CGInsn.Build<ModifyStack>(new CGImmediate<ushort>((ushort)Scope.StackUsed)));
			if (regs != 0) gen.Add(CGInsn.Build<PopRegisters>(new CGImmediate<ushort>(regs)));
		}
	}

	public class Memset(IPointer ptr, int size, byte b) : IRInsn
	{
		public readonly IPointer Ptr = ptr;
		public readonly int Size = size;
		public readonly byte Byte = b;

		public override void Compile(CodeGen gen)
		{
			if (Size % 2 != 0) throw new NotImplementedException();

			gen.Add(CGInsn.Build<Load>(new CGRegister(8), new CGImmediate<ushort>((ushort)(Byte & (Byte << 8)))));
			for (int i = 0; i < Size / 2; i++)
			{
				gen.Add(CGInsn.Build<Store>(Ptr.Build<ushort>(Scope), new CGRegister(8)));
			}
		}
	}

	public class SignExtPtr(IPointer dest, IPointer src) : IRInsn
	{
		public readonly IPointer Dest = dest;
		public readonly IPointer Src = src;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<SignExtend>(Dest.Build<ushort>(Scope), Src.Build<ushort>(Scope)));
		}
	}

	public class SignExtReg(int dest, int src) : IRInsn
	{
		public readonly int Dest = dest;
		public readonly int Src = src;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<SignExtend>(new CGRegister(Dest), new CGRegister(Src)));
		}
	}

	public class SignExtReg8(int dest, int src) : IRInsn
	{
		public readonly int Dest = dest;
		public readonly int Src = src;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<SignExtend8>(new CGRegister(Dest), new CGRegister(Src)));
		}
	}

	public class IRLabel(string name) : IRInsn
	{
		public readonly string Name = name;

        public override void Compile(CodeGen gen)
        {
			gen.AddLabel(Name);
        }
    }
}
