using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dankle.Components.Arguments;

namespace Dankle.Components.Instructions
{
	public abstract class Instruction
	{
		private static readonly Dictionary<ushort, Instruction> Instructions = [];

		public abstract ushort Opcode { get; }

		// Used by the assembler
		public abstract Type[] Arguments { get; }
		public abstract string Name { get; }

		public static Context GetNextContext(CPUCore core)
		{
			var info = core.GetNext();
			var data = new byte[4];

			data[3] = (byte)(info & 0x000F);
			data[2] = (byte)((info >>> 4) & 0x000F);
			data[1] = (byte)((info >>> 8) & 0x000F);
			data[0] = (byte)((info >>> 12) & 0x000F);

			return new(core, data);
		}

		public void Execute(CPUCore core)
		{
			if (core.Computer.Debug)
			{
				Console.WriteLine($"0x{core.ProgramCounter - 2:X8}: {core.Dissassemble(core.ProgramCounter - 2)}");
			}

			Handle(GetNextContext(core));
		}

		protected abstract void Handle(Context ctx);

		public static void Register<T>() where T : Instruction, new()
		{
			var insn = new T();
			if (Instructions.ContainsKey(insn.Opcode)) throw new InvalidOperationException();
			Instructions[insn.Opcode] = insn;
		}

		public static Instruction Get(ushort opcode)
		{
			if (Instructions.TryGetValue(opcode, out var insn)) return insn;
			throw new ArgumentException($"Unknown opcode 0x{opcode:X4}");
		}

		public static Instruction Get(string name)
		{
			foreach (var i in Instructions.Values)
			{
				if (i.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)) return i;
			}

			throw new ArgumentException($"Unknown instruction {name}");
		}

		static Instruction()
		{
			Register<Halt>();
			Register<Noop>();
			Register<Load>();
			Register<Load8>();
			Register<Store>();
			Register<Store8>();
			Register<Move>();
			Register<Add>();
			Register<Subtract>();
			Register<SignedMul>();
			Register<SignedDiv>();
			Register<UnsignedMul>();
			Register<UnsignedDiv>();
			Register<LeftShift>();
			Register<RightShift>();
			Register<Compare>();
			Register<LessThan>();
			Register<LessThanOrEq>();
			Register<GreaterThan>();
			Register<GreaterThanOrEq>();
			Register<Jump>();
			Register<JumpEq>();
			Register<JumpNeq>();
			Register<JumpZ>();
			Register<JumpNZ>();
			Register<Or>();
			Register<And>();
			Register<Xor>();
			Register<Modulo>();
			Register<Call>();
			Register<Return>();
			Register<Push>();
			Register<Pop>();
			Register<Increment>();
			Register<Decrement>();
			Register<Adc>();
			Register<Low>();
			Register<High>();
			Register<Reset>();
			Register<Sbb>();
			Register<UnsignedDiv32>();
			Register<Modulo32>();
			Register<CompareFlags>();
			Register<CompareFlagsEven>();
			Register<CompareFlagsOdd>();
			Register<PushFlags>();
			Register<PopFlags>();
			Register<SignedDiv32>();
			Register<SignedModulo32>();
			Register<LE>();
			Register<BE>();
			Register<Negate>();
			Register<XCall>();
			Register<PushRegisters>();
			Register<PopRegisters>();
			Register<ModifyStack>();
			Register<SignExtend>();
			Register<SignExtend8>();
			Register<SignedMul32>();
			Register<UnsignedMul32>();
			Register<LoadEffectiveAddress>();
			Register<GetCompare>();
			Register<GetNotCompare>();
			Register<UnsignedLessThan>();
			Register<UnsignedLessThanOrEq>();
			Register<UnsignedGreaterThan>();
			Register<UnsignedGreaterThanOrEq>();
			Register<LessThan32>();
			Register<LessThanOrEq32>();
			Register<GreaterThan32>();
			Register<GreaterThanOrEq32>();
			Register<UnsignedLessThan32>();
			Register<UnsignedLessThanOrEq32>();
			Register<UnsignedGreaterThan32>();
			Register<UnsignedGreaterThanOrEq32>();
			Register<FlipCompare>();
			Register<SignedMul64>();
			Register<UnsignedMul64>();
			Register<SignedDiv64>();
			Register<UnsignedDiv64>();
			Register<LessThan64>();
			Register<LessThanOrEq64>();
			Register<GreaterThan64>();
			Register<GreaterThanOrEq64>();
			Register<UnsignedLessThan64>();
			Register<UnsignedLessThanOrEq64>();
			Register<UnsignedGreaterThan64>();
			Register<UnsignedGreaterThanOrEq64>();
			Register<Load32>();
			Register<Store32>();
			Register<Load64>();
			Register<Store64>();
			Register<SignedModulo>();
			Register<SignedModulo64>();
			Register<Modulo64>();
			Register<LeftShift32>();
			Register<LeftShift64>();
			Register<RightShift32>();
			Register<RightShift64>();
			Register<ArithmeticRightShift>();
			Register<ArithmeticRightShift32>();
			Register<ArithmeticRightShift64>();
		}
	}

	public class Context(CPUCore core, byte[] data)
	{
		public readonly byte[] Data = data;
		public readonly CPUCore Core = core;

		private int ArgIndex = 0;

		public T GetNextArg<T>() where T : IArgument, new() => (T)new T().Create(this, ArgIndex++);
	}
}
