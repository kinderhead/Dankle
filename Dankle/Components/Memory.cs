using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class Memory(Computer computer, long size) : Component(computer)
	{
		public override string Name => "Memory";

		private readonly byte[] Data = new byte[size];

		protected override void Init()
		{
			RegisterHandler((MemoryReadMsg i) => Read(i.Address));
			RegisterHandler((MemoryWriteMsg i) => Write(i.Address, i.Value));
		}

		private byte Read(uint addr)
		{
			if (addr >= Data.Length) throw new ArgumentException($"Invalid address {addr:X}");
			return Data[addr];
		}

		private bool Write(uint addr, byte val)
		{
			if (addr >= Data.Length) throw new ArgumentException($"Invalid address {addr:X}");
			Data[addr] = val;
			return true;
		}
	}

	public class MemoryReadMsg : Message<byte>
	{
		public required uint Address;
	}

	public class MemoryWriteMsg : Message<bool>
	{
		public required uint Address;
		public required byte Value;
	}
}
