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
			if (Register == -1) return;
			gen.Add(CGInsn.Build<Load>(new CGRegister(Register), new CGImmediate<ushort>(Val)));
		}
	}

	public class LoadPtrToReg(int reg, IPointer pointer) : IRInsn
	{
		public readonly int Register = reg;
		public readonly IPointer Pointer = pointer;

		public override void Compile(CodeGen gen)
		{
			if (Register == -1) return;
			gen.Add(CGInsn.Build<Load>(new CGRegister(Register), Pointer.Build<ushort>(Scope)));
		}
	}

	public class LoadPtrToReg8(int reg, IPointer pointer) : IRInsn
	{
		public readonly int Register = reg;
		public readonly IPointer Pointer = pointer;

		public override void Compile(CodeGen gen)
		{
			if (Register == -1) return;
			gen.Add(CGInsn.Build<Load8>(new CGRegister(Register), Pointer.Build<byte>(Scope)));
		}
	}

	public class LoadRegToPtr(IPointer pointer, int reg) : IRInsn
	{
		public readonly int Register = reg;
		public readonly IPointer Pointer = pointer;

		public override void Compile(CodeGen gen)
		{
			if (Register == -1) return;
			gen.Add(CGInsn.Build<Store>(Pointer.Build<ushort>(Scope), new CGRegister(Register)));
		}
	}

	public class LoadRegToPtr8(IPointer pointer, int reg) : IRInsn
	{
		public readonly int Register = reg;
		public readonly IPointer Pointer = pointer;

		public override void Compile(CodeGen gen)
		{
			if (Register == -1) return;
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

	public class SubRegs(int arg1, int arg2, int dest) : IRInsn
	{
		public readonly int Arg1 = arg1;
		public readonly int Arg2 = arg2;
		public readonly int Dest = dest;

		public override void Compile(CodeGen gen)
		{
			if (Arg1 == -1 || Arg2 == -1 || Dest == -1) return;
			gen.Add(CGInsn.Build<Subtract>(new CGRegister(Arg1), new CGRegister(Arg2), new CGRegister(Dest)));
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

	public class SbbRegs(int arg1, int arg2, int dest) : IRInsn
	{
		public readonly int Arg1 = arg1;
		public readonly int Arg2 = arg2;
		public readonly int Dest = dest;

		public override void Compile(CodeGen gen)
		{
			if (Arg1 == -1 || Arg2 == -1 || Dest == -1) return;
			gen.Add(CGInsn.Build<Sbb>(new CGRegister(Arg1), new CGRegister(Arg2), new CGRegister(Dest)));
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

	public class SDivRegs(int arg1, int arg2, int dest) : IRInsn
	{
		public readonly int Arg1 = arg1;
		public readonly int Arg2 = arg2;
		public readonly int Dest = dest;

		public override void Compile(CodeGen gen)
		{
			if (Arg1 == -1 || Arg2 == -1 || Dest == -1) return;
			gen.Add(CGInsn.Build<SignedDiv>(new CGRegister(Arg1), new CGRegister(Arg2), new CGRegister(Dest)));
		}
	}

	public class UDivRegs(int arg1, int arg2, int dest) : IRInsn
	{
		public readonly int Arg1 = arg1;
		public readonly int Arg2 = arg2;
		public readonly int Dest = dest;

		public override void Compile(CodeGen gen)
		{
			if (Arg1 == -1 || Arg2 == -1 || Dest == -1) return;
			gen.Add(CGInsn.Build<UnsignedDiv>(new CGRegister(Arg1), new CGRegister(Arg2), new CGRegister(Dest)));
		}
	}

	public class SMul32Regs(int arg1high, int arg1low, int arg2high, int arg2low, int desthigh, int destlow) : IRInsn
	{
		public readonly int Arg1High = arg1high;
		public readonly int Arg1Low = arg1low;
		public readonly int Arg2High = arg2high;
		public readonly int Arg2Low = arg2low;
		public readonly int DestHigh = desthigh;
		public readonly int DestLow = destlow;

		public override void Compile(CodeGen gen)
		{
			if (Arg1High == -1 || Arg1Low == -1 || Arg2High == -1 || Arg2Low == -1 || DestHigh == -1 || DestLow == -1) return;
			gen.Add(CGInsn.Build<SignedMul32>(new CGDoubleRegister(Arg1High, Arg1Low), new CGDoubleRegister(Arg2High, Arg2Low), new CGDoubleRegister(DestHigh, DestLow)));
		}
	}

	public class UMul32Regs(int arg1high, int arg1low, int arg2high, int arg2low, int desthigh, int destlow) : IRInsn
	{
		public readonly int Arg1High = arg1high;
		public readonly int Arg1Low = arg1low;
		public readonly int Arg2High = arg2high;
		public readonly int Arg2Low = arg2low;
		public readonly int DestHigh = desthigh;
		public readonly int DestLow = destlow;

		public override void Compile(CodeGen gen)
		{
			if (Arg1High == -1 || Arg1Low == -1 || Arg2High == -1 || Arg2Low == -1 || DestHigh == -1 || DestLow == -1) return;
			gen.Add(CGInsn.Build<UnsignedMul32>(new CGDoubleRegister(Arg1High, Arg1Low), new CGDoubleRegister(Arg2High, Arg2Low), new CGDoubleRegister(DestHigh, DestLow)));
		}
	}

	public class SDiv32Regs(int arg1high, int arg1low, int arg2high, int arg2low, int desthigh, int destlow) : IRInsn
	{
		public readonly int Arg1High = arg1high;
		public readonly int Arg1Low = arg1low;
		public readonly int Arg2High = arg2high;
		public readonly int Arg2Low = arg2low;
		public readonly int DestHigh = desthigh;
		public readonly int DestLow = destlow;

		public override void Compile(CodeGen gen)
		{
			if (Arg1High == -1 || Arg1Low == -1 || Arg2High == -1 || Arg2Low == -1 || DestHigh == -1 || DestLow == -1) return;
			gen.Add(CGInsn.Build<SignedDiv32>(new CGDoubleRegister(Arg1High, Arg1Low), new CGDoubleRegister(Arg2High, Arg2Low), new CGDoubleRegister(DestHigh, DestLow)));
		}
	}

	public class UDiv32Regs(int arg1high, int arg1low, int arg2high, int arg2low, int desthigh, int destlow) : IRInsn
	{
		public readonly int Arg1High = arg1high;
		public readonly int Arg1Low = arg1low;
		public readonly int Arg2High = arg2high;
		public readonly int Arg2Low = arg2low;
		public readonly int DestHigh = desthigh;
		public readonly int DestLow = destlow;

		public override void Compile(CodeGen gen)
		{
			if (Arg1High == -1 || Arg1Low == -1 || Arg2High == -1 || Arg2Low == -1 || DestHigh == -1 || DestLow == -1) return;
			gen.Add(CGInsn.Build<UnsignedDiv32>(new CGDoubleRegister(Arg1High, Arg1Low), new CGDoubleRegister(Arg2High, Arg2Low), new CGDoubleRegister(DestHigh, DestLow)));
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
			if (Scope.EffectiveStackUsed != 0) gen.Add(CGInsn.Build<ModifyStack>(new CGImmediate<ushort>((ushort)-Scope.EffectiveStackUsed)));
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

			if (Scope.EffectiveStackUsed != 0) gen.Add(CGInsn.Build<ModifyStack>(new CGImmediate<ushort>((ushort)Scope.EffectiveStackUsed)));
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
			gen.Add(CGInsn.Build<Load>(new CGRegister(8), new CGImmediate<ushort>((ushort)(Byte & (Byte << 8)))));

			for (int i = 0; i < IRBuilder.NumRegForBytes(Size); i++)
			{
				if ((i + 1) * 2 > Size) gen.Add(CGInsn.Build<Store8>(Ptr.Get(i * 2).Build<byte>(Scope), new CGRegister(8)));
				else gen.Add(CGInsn.Build<Store>(Ptr.Get(i * 2).Build<ushort>(Scope), new CGRegister(8)));
			}
		}
	}

	public class SignExtPtr(IPointer dest, IPointer src) : IRInsn
	{
		public readonly IPointer Dest = dest;
		public readonly IPointer Src = src;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<SignExtend>(Src.Build<ushort>(Scope), Dest.Build<ushort>(Scope)));
		}
	}

	public class SignExtPtr8(IPointer dest, IPointer src) : IRInsn
	{
		public readonly IPointer Dest = dest;
		public readonly IPointer Src = src;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<SignExtend8>(Src.Build<ushort>(Scope), Dest.Build<ushort>(Scope)));
		}
	}

	public class SignExtReg(int dest, int src) : IRInsn
	{
		public readonly int Dest = dest;
		public readonly int Src = src;

		public override void Compile(CodeGen gen)
		{
			if (Dest == -1 || Src == -1) return;
			gen.Add(CGInsn.Build<SignExtend>(new CGRegister(Src), new CGRegister(Dest)));
		}
	}

	public class SignExtReg8(int dest, int src) : IRInsn
	{
		public readonly int Dest = dest;
		public readonly int Src = src;

		public override void Compile(CodeGen gen)
		{
			if (Dest == -1 || Src == -1) return;
			gen.Add(CGInsn.Build<SignExtend8>(new CGRegister(Src), new CGRegister(Dest)));
		}
	}

	public class LeaReg(int dest1, int dest2, IPointer src) : IRInsn
	{
		public readonly int Dest1 = dest1;
		public readonly int Dest2 = dest2;
		public readonly IPointer Source = src;

		public override void Compile(CodeGen gen)
		{
			if (Dest1 == -1 || Dest2 == -1) return;
			gen.Add(CGInsn.Build<LoadEffectiveAddress>(new CGDoubleRegister(Dest1, Dest2), Source.Build<ushort>(Scope)));
		}
	}

	public class LeaPtr(IPointer dest, IPointer src) : IRInsn
	{
		public readonly IPointer Dest = dest;
		public readonly IPointer Source = src;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CGInsn.Build<LoadEffectiveAddress>(Dest.Build<uint>(Scope), Source.Build<ushort>(Scope)));
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
