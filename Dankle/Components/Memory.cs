using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class Memory : Component
	{
		public override string Name => "Memory";

		private readonly byte[] Data;

        public Memory(Computer computer, long size) : base(computer)
        {
            Data = new byte[size];
			
			RegisterHandler((MemoryReadMsg i) => HandleRead(i.Address));
			RegisterHandler((MemoryWriteMsg i) => HandleWrite(i.Address, i.Value));
		}

        public byte Read(uint addr, Component? source = null) => Send<MemoryReadMsg, byte>(new MemoryReadMsg { Address = addr, Source = source });
		public ushort Read16(uint addr, Component? source = null) => Utils.Merge(Read(addr, source), Read(addr + 1, source));

		public bool Write(uint addr, byte val, Component? source = null) => Send<MemoryWriteMsg, bool>(new MemoryWriteMsg { Address = addr, Value = val, Source = source });

		private byte HandleRead(uint addr)
		{
			if (addr >= Data.Length) throw new ArgumentException($"Invalid address {addr:X}");
			return Data[addr];
		}

		private bool HandleWrite(uint addr, byte val)
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
