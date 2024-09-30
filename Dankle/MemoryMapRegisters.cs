using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dankle
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ReadRegister(uint offset, uint size = 1) : Attribute
	{
		public readonly uint AddressOffset = offset;
		public readonly uint Size = size;
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class WriteRegister(uint offset, uint size = 1) : Attribute
	{
		public readonly uint AddressOffset = offset;
		public readonly uint Size = size;
	}

	public abstract class MemoryMapRegisters
	{
		private readonly Dictionary<uint, HandlerEntry> Handlers = [];

		public readonly uint Address;

		private class HandlerEntry : MemoryMapEntry
		{
			public uint AddressOffset;
			public uint Size;
			public Func<uint, byte[]>? ReadHandler;
			public Action<uint, byte[]>? WriteHandler;

			public MemoryMapRegisters Parent;

			public override uint StartAddr => Parent.Address + AddressOffset;
			public override uint EndAddr => StartAddr + Size;

			public override byte[] Read(uint addr, uint size)
			{
				if (ReadHandler is null || StartAddr != addr || Size != size) throw new InvalidOperationException($"Cannot read from register at 0x{addr:X4}");
				return ReadHandler(addr);
			}

			public override void Write(uint addr, byte[] data)
			{
				if (WriteHandler is null || StartAddr != addr || Size != data.Length) throw new InvalidOperationException($"Cannot write to register at 0x{addr:X4}");
				WriteHandler(addr, data);
			}
		}

		public MemoryMapRegisters(uint addr)
		{
			Address = addr;

			foreach (var i in GetType().GetMethods())
			{
				var readAttr = i.GetCustomAttribute<ReadRegister>();
				if (readAttr is not null)
				{
					if (Handlers.TryGetValue(readAttr.AddressOffset, out var handler)) handler.ReadHandler = i.CreateDelegate<Func<uint, byte[]>>(this);
					else Handlers[readAttr.AddressOffset] = new HandlerEntry { Parent = this, AddressOffset = readAttr.AddressOffset, Size = readAttr.Size, ReadHandler = i.CreateDelegate<Func<uint, byte[]>>(this) };
				}

				var writeAttr = i.GetCustomAttribute<WriteRegister>();
				if (writeAttr is not null)
				{
					if (Handlers.TryGetValue(writeAttr.AddressOffset, out var handler)) handler.WriteHandler = i.CreateDelegate<Action<uint, byte[]>>(this);
					else Handlers[writeAttr.AddressOffset] = new HandlerEntry { Parent = this, AddressOffset = writeAttr.AddressOffset, Size = writeAttr.Size, WriteHandler = i.CreateDelegate<Action<uint, byte[]>>(this) };
				}
			}
		}

		public void Register(Computer computer)
		{
			foreach (var i in Handlers)
			{
				computer.AddMemoryMapEntry(i.Value);
			}
		}
	}
}
