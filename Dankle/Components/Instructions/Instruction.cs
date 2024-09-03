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

		protected class Context
		{
			public readonly byte[] Data;
			public readonly CPUCore Core;
			public readonly Func<ushort> Supply;

			private int ArgIndex = 0;

			public T Arg<T>() where T : IArgument => (T?)Activator.CreateInstance(typeof(T), Core, Data[ArgIndex++], Supply) ?? throw new ArgumentException($"Invalid argument type {typeof(T).Name}");
		}
	}
}
