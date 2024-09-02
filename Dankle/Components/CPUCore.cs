using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class CPUCore(Computer computer) : Component(computer)
	{
		public override string Name => "CPUCore";

		private readonly ushort[] Registers = new ushort[16];

		private ushort ProgramCounter => Registers[15];
		private ushort StackPointer => Registers[14];

		protected override void Process()
		{
			while (!ShouldStop)
			{
				HandleMessage(false);
			}
		}
	}
}
