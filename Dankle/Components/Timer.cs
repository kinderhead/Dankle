using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class Timer : Component
	{
		public override string Name => "Timer";

		public Timer(Computer computer, uint addr) : base(computer)
		{
			computer.AddMemoryMapEntry(new MM(addr));
		}

		public class MM(uint addr) : MemoryMapRegisters(addr)
		{
			[WriteRegister(0, 2)]
			public void Sleep(uint _, byte[] data) => Thread.Sleep(Utils.FromBytes<ushort>(data));
		}
	}
}
