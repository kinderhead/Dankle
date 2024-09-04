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

		public void Execute(CPUCore core, Func<ushort> supply)
		{
			var info = supply();
			var data = new byte[4];

			data[0] = (byte)(info & 0x000F);
			data[1] = (byte)((info >> 4) & 0x000F);
			data[2] = (byte)((info >> 8) & 0x000F);
			data[3] = (byte)((info >> 12) & 0x000F);

			Handle(new(core, data, supply));
		}

		protected abstract void Handle(Context ctx);

		public static void Register<T>() where T : Instruction, new()
		{
			var insn = new T();
			Instructions[insn.Opcode] = insn;
		}

		public static Instruction Get(ushort opcode)
		{
			if (Instructions.TryGetValue(opcode, out var insn)) return insn;
			throw new ArgumentException($"Unknown opcode {opcode:X}");
		}

		protected class Context(CPUCore core, byte[] data, Func<ushort> supply)
		{
			public readonly byte[] Data = data;
			public readonly CPUCore Core = core;
			public readonly Func<ushort> Supply = supply;

			private int ArgIndex = 0;

			public T Arg<T>() where T : IArgument => (T?)Activator.CreateInstance(typeof(T), Core, Data[ArgIndex++], Supply) ?? throw new ArgumentException($"Invalid argument type {typeof(T).Name}");
		}

		static Instruction()
		{
			Register<Halt>();
		}
	}
}
