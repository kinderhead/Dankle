using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class Debugger : Component
	{
		public override string Name => "Debugger";

		public Debugger(Computer computer, uint addr) : base(computer)
		{
			computer.AddMemoryMapEntry(new MM(addr, this));
		}

		public void Break()
		{
			Computer.StartDebugAsTask();
		}

		public class MM(uint addr, Debugger db) : MemoryMapRegisters(addr)
		{
			public readonly Debugger Debugger = db;

			[WriteRegister(0, 1)]
			public void Break(uint _, byte[] __)
			{
				Debugger.Break();
			}
		}
	}
}
