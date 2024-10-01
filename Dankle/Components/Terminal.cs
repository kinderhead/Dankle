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
			computer.AddMemoryMapEntry(new MM(addr, this));
		}

		public virtual void WriteOut(string text)
		{
			Console.Write(text);
			Console.Out.Flush();
		}

		public class MM(uint addr, Terminal term) : MemoryMapRegisters(addr)
		{
			public readonly Terminal Terminal = term;

			[WriteRegister(0)]
			public void WriteOut(uint _, byte[] data)
			{
				Terminal.WriteOut(Encoding.UTF8.GetString(data));
			}
		}
	}
}
