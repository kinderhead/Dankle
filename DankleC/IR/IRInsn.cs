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

		public readonly List<InsnDef> Insns = [];

		public abstract void Compile(CodeGen gen);
		public virtual void PostCompile(CodeGen gen) { }

		public int Alloc()
		{
			var reg = OneTimeAlloc();
			usedRegs.Add(reg);
			return reg;
		}

		public int[] Alloc(int bytes)
		{
			List<int> ret = [];

			for (int i = 0; i < IRBuilder.NumRegForBytes(bytes); i++)
			{
				var reg = OneTimeAlloc();
				usedRegs.Add(reg);
				ret.Add(reg);
			}

			return [.. ret];
		}

		public int OneTimeAlloc()
		{
			for (int i = 4; i < 12; i++)
			{
				if (!usedRegs.Contains(i))
				{
					return i;
				}
			}

			throw new InvalidOperationException();
		}

		public void Free(int reg) => usedRegs.Remove(reg);

		public void Free(int[] regs)
		{
			foreach (var i in regs)
			{
				Free(i);
			}
		}

		public void Add(CGInsn insn)
		{
			Insns.Add(new(insn));
			insn.Comment = GetType().Name;
		}
		
		public void Add(string label) => Insns.Add(new(label));

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

		public void MovePtrToRegs(IPointer ptr, int[] regs)
		{
			if (regs.Length != IRBuilder.NumRegForBytes(ptr.Size)) throw new InvalidOperationException();
			else if (ptr.Size == 1) Add(CGInsn.Build<Load8>(new CGRegister(regs[0]), ptr.Build<byte>(Scope)));
			else if (ptr.Size % 2 == 0)
			{
				List<(int, int)> temps = [];
				for (var i = 0; i < ptr.Size; i += 2)
				{
					var reg = regs[i / 2];
					if (ptr.UsingRegister(reg) && i != ptr.Size - 2)
					{
						reg = Alloc();
						temps.Add((reg, regs[i / 2]));
					}
					Add(CGInsn.Build<Load>(new CGRegister(reg), ptr.Get(i).Build<ushort>(Scope)));
				}

				foreach (var i in temps)
				{
					Add(CGInsn.Build<Move>(new CGRegister(i.Item2), new CGRegister(i.Item1)));
				}
			}
			else throw new InvalidOperationException();
		}

		public void MoveRegsToRegs(int[] src, int[] dest)
		{
			if (src.Length != dest.Length) throw new InvalidOperationException();

			for (int i = 0; i < src.Length; i++)
			{
				Add(CGInsn.Build<Move>(new CGRegister(dest[i]), new CGRegister(src[i])));
			}
		}

		public void MoveRegsToRegsReversed(int[] src, int[] dest)
		{
			if (src.Length != dest.Length) throw new InvalidOperationException();

			for (int i = src.Length - 1; i >= 0; i--)
			{
				Add(CGInsn.Build<Move>(new CGRegister(dest[i]), new CGRegister(src[i])));
			}
		}

		protected void Return(IValue value)
		{
			var regs = FitRetRegs(value.Type.Size);
			value.WriteTo(this, regs);
		}

		public static int[] FitRetRegs(int bytes)
		{
			var regs = IRBuilder.NumRegForBytes(bytes);
			if (regs == 1) return [0];
			if (regs == 2) return [0, 1];
			if (regs == 3) return [0, 1, 2];
			if (regs == 4) return [0, 1, 2, 3];
			throw new InvalidOperationException();
		}

		public static SimpleRegisterValue GetReturn(TypeSpecifier type) => new(FitRetRegs(type.Size), type);
	}

	public class IRStorePtr(IPointer ptr, IValue value) : IRInsn
	{
		public readonly IPointer Ptr = ptr;
		public readonly IValue Value = value;

		public override void Compile(CodeGen gen)
		{
			Value.WriteTo(this, Ptr);
		}
	}

	public class IRStoreRegs(int[] regs, IValue value) : IRInsn
	{
		public readonly int[] Registers = regs;
		public readonly IValue Value = value;

		public override void Compile(CodeGen gen)
		{
			Value.WriteTo(this, Registers);
		}
	}

	public class IRSetReturn(IValue value) : IRInsn
	{
		public readonly IValue Value = value;

		public override void Compile(CodeGen gen)
		{
			Return(Value);
		}
	}

	public class IRReturnFunc : IRInsn
	{
		public override void Compile(CodeGen gen)
		{
			Add(CGInsn.Build<Return>());
		}
	}

	public class IRDynLoadPtr(IValue ptr, TypeSpecifier type) : IRInsn
	{
		public readonly IValue Pointer = ptr;
		public readonly TypeSpecifier Type = type;

		public override void Compile(CodeGen gen)
		{
			var regs = GetReturn(Type);
			var ptrRegs = Pointer.ToRegisters(this);
			MovePtrToRegs(new RegisterPointer(ptrRegs.Registers[0], ptrRegs.Registers[1], 0, Type.Size), regs.Registers);
		}
	}

	public class IRDynStorePtr(IValue ptr, IValue value) : IRInsn
	{
		public readonly IValue Pointer = ptr;
		public readonly IValue Value = value;

		public override void Compile(CodeGen gen)
		{
			if (Pointer is Immediate32 i)
			{
				Value.WriteTo(this, new LiteralPointer(i.Value, Value.Type.Size));
			}
			else
			{
				var ptrRegs = Pointer.ToRegisters(this);
				Value.WriteTo(this, new RegisterPointer(ptrRegs.Registers[0], ptrRegs.Registers[1], 0, Value.Type.Size));
			}
		}
	}

	public class IRLoadPtrAddress(IPointer ptr) : IRInsn
	{
		public readonly IPointer Pointer = ptr;

		public override void Compile(CodeGen gen)
		{
			var regs = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedInt));
			Add(CGInsn.Build<LoadEffectiveAddress>(Pointer.Build<ushort>(Scope), regs.MakeArg()));
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
