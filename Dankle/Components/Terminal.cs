using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class Terminal : Component
	{
		public override string Name => "Terminal";

		public Terminal(Computer computer, uint addr) : base(computer)
		{
			computer.AddMemoryMapEntry(new MM(addr));
		}

		public class MM(uint addr) : MemoryMapRegisters(addr)
		{
			[WriteRegister(0)]
			public void WriteOut(uint _, byte[] data)
			{
				Console.Write(Encoding.UTF8.GetString(data));
				Console.Out.Flush();
			}
		}
	}
}
