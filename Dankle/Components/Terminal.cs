using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class Terminal : Component
	{
		public override string Name => "Terminal";
		public uint Addr;

		public StringBuilder? Builder;

		public Terminal(Computer computer, uint addr) : base(computer)
		{
			Addr = addr;
			computer.AddMemoryMapEntry(new MM(addr, this));
		}

		public Terminal(Computer computer, uint addr, StringBuilder builder) : base(computer)
		{
			Addr = addr;
			computer.AddMemoryMapEntry(new MM(addr, this));
			Builder = builder;
		}

		public virtual void WriteOut(string text)
		{
			Console.Write(text);
			Console.Out.Flush();
			Builder?.Append(text);
		}

		public class MM(uint addr, Terminal term) : MemoryMapRegisters(addr)
		{
			public readonly Terminal Terminal = term;

			[WriteRegister(0)]
			public void WriteOut(uint _, byte[] data)
			{
				Terminal.WriteOut(Encoding.UTF8.GetString(data));
			}

			[ReadRegister(1)]
			public byte[] Read(uint _)
			{
				char c = Console.ReadKey().KeyChar;

				if (c == '\r') return [10]; // Enter
				else if (c == '\b' || c == 127) return [127]; // Backspace

				return [Encoding.UTF8.GetBytes([c])[0]];
			}
		}
	}

	public class Terminal16 : Component
	{
		public override string Name => "Terminal";
		public uint Addr;

		public Terminal16(Computer computer, uint addr) : base(computer)
		{
			Addr = addr;
			computer.AddMemoryMapEntry(new MM(addr, this));
		}

		public virtual void WriteOut(string text)
		{
			Console.Write(text);
			//Console.Out.Flush();
		}

		public class MM(uint addr, Terminal16 term) : MemoryMapRegisters(addr)
		{
			public readonly Terminal16 Terminal = term;

			[WriteRegister(0, 2)]
			public void WriteOut(uint _, byte[] data)
			{
				Terminal.WriteOut(Encoding.UTF8.GetString(data[1..2]));
			}
		}
	}
}
