using Dankle.Components.Instructions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class CPUCore(Computer computer) : Component(computer)
	{
		public override string Name => "CPU Core";

		public readonly ConcurrentDictionary<int, ushort> Registers = new(Enumerable.Range(0, 16).ToDictionary(i => i, i => (ushort)0));

		public ushort ProgramCounter { get => Registers[15]; set => Registers[15] = value; }
		public ushort StackPointer { get => Registers[14]; set => Registers[14] = value; }

		protected override void Process()
		{
			while (!ShouldStop)
			{
				HandleMessage(false);
				Cycle();
			}
		}

		protected ushort GetNextWord()
		{
			var val = Computer.ReadMem16(ProgramCounter);
			ProgramCounter += 2;
			return val;
		}

		private void Cycle()
		{
			var op = GetNextWord();
			Instruction.Get(op).Execute(this, GetNextWord);
		}
	}
}
