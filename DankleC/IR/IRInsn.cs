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

		private readonly List<int> usedRegs = [];

		public readonly List<CGInsn> Insns = [];

		public abstract void Compile(CodeGen gen);
		public virtual void PostCompile(CodeGen gen) { }

		public int Alloc()
		{
			var reg = OneTimeAlloc();
			usedRegs.Add(reg);
			return reg;
		}

		public int OneTimeAlloc()
		{
			for (int i = 8; i < 12; i++)
			{
				if (!usedRegs.Contains(i))
				{
					return i;
				}
			}

			throw new InvalidOperationException();
		}

		public void Free(int reg) => usedRegs.Remove(reg);
		public void Add(CGInsn insn) => Insns.Add(insn);

		public void MoveRegsToPtr(int[] regs, IPointer ptr)
		{
			if (regs.Length != IRBuilder.NumRegForBytes(ptr.Size)) throw new InvalidOperationException();
			else if (ptr.Size == 1) Add(CGInsn.Build<Store8>(ptr.Build<byte>(Scope), new CGRegister(regs[0])));
			else if (ptr.Size % 2 == 0)
			{
				for (var i = 0; i < ptr.Size; i += 2)
				{
					Add(CGInsn.Build<Store>(ptr.Get(i).Build<ushort>(Scope), new CGRegister(regs[i / 2])));
				}
			}
			else throw new InvalidOperationException();
		}

		public void MovePtrToPtr(IPointer src, IPointer dest)
		{
			if (src.Size > dest.Size) throw new InvalidOperationException();
			else
			{
				var reg = OneTimeAlloc();
				for (var i = 0; i < IRBuilder.NumRegForBytes(src.Size); i++)
				{
					if ((i + 1) * 2 > src.Size)
					{

						Add(CGInsn.Build<Load8>(new CGRegister(reg), src.Get(i * 2).Build<byte>(Scope)));
						Add(CGInsn.Build<Store8>(dest.Get(i * 2).Build<byte>(Scope), new CGRegister(reg)));
					}
					else
					{
						Add(CGInsn.Build<Load>(new CGRegister(reg), src.Get(i * 2).Build<ushort>(Scope)));
						Add(CGInsn.Build<Store>(dest.Get(i * 2).Build<ushort>(Scope), new CGRegister(reg)));
					}
				}
			}
		}
	}

    public class IRStore(IPointer ptr, IValue value) : IRInsn
    {
		public readonly IPointer Ptr = ptr;
		public readonly IValue Value = value;

        public override void Compile(CodeGen gen)
		{
			Value.WriteTo(this, Ptr);
		}
    }

    public class InitFrame() : IRInsn
	{
		public override void Compile(CodeGen gen) { }

        public override void PostCompile(CodeGen gen)
		{
			if (Scope.EffectiveStackUsed != 0) Add(CGInsn.Build<ModifyStack>(new CGImmediate<ushort>((ushort)-Scope.EffectiveStackUsed)));
		}
	}

	public class EndFrame() : IRInsn
	{
		public override void Compile(CodeGen gen) { }

		public override void PostCompile(CodeGen gen)
		{
			if (Scope.EffectiveStackUsed != 0) Add(CGInsn.Build<ModifyStack>(new CGImmediate<ushort>((ushort)Scope.EffectiveStackUsed)));
		}
	}
}
