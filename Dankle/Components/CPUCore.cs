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

		public ushort ProgramCounter => Registers[15];
		public ushort StackPointer => Registers[14];

		protected override void Process()
		{
			while (!ShouldStop)
			{
				HandleMessage(false);
			}
		}
	}
}
