using Dankle.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dankle
{
	public class Computer : IDisposable
	{
		private readonly List<Component> Components = [];
		private readonly List<MemoryMapEntry> MemoryMap = [];

		public bool Started { get; private set; }
		public bool StoppingOrStopped { get; private set; }

		public readonly uint MemorySize;

		public bool Debug = false;

		public Computer(uint memSize)
		{
			MemorySize = memSize;
			AddMemoryMapEntry(new RAM(memSize));
			AddComponent<CPUCore>();
		}

		public void AddComponent<T>(params object[] args) where T : Component
		{
			var comp = (T?)Activator.CreateInstance(typeof(T), [this, ..args]) ?? throw new ArgumentException("Could not create component");
			AddComponent(comp);
		}

		public void AddComponent(Component comp)
		{
			Components.Add(comp);
		}

		public T GetComponent<T>() where T : Component
		{
			foreach (var i in Components)
			{
				if (i is T comp) return comp;
			}

			throw new ArgumentException($"Could not find object with type {typeof(T).Name}");
		}

		public void AddMemoryMapEntry(MemoryMapEntry entry)
		{
			MemoryMap.Add(entry);
			MemoryMap.Sort();
		}

		public void AddMemoryMapEntry(MemoryMapRegisters entry)
		{
			entry.Register(this);
		}

		public OrderedDictionary<MemoryMapEntry, (uint startAddr, uint startIndex, uint size)> GetMemoryMapsForRange(uint addr, uint size)
		{
			var entries = new OrderedDictionary<MemoryMapEntry, (uint startAddr, uint startIndex, uint size)>();

			uint originalAddr = addr;
			uint originalSize = size;
			uint index = 0;

			foreach (var i in MemoryMap)
			{
				if (addr >= i.StartAddr && addr < i.EndAddr)
				{
					if (addr + size <= i.EndAddr)
					{
						entries.Add(i, (addr, index, size));
						return entries;
					}
					entries.Add(i, (addr, index, i.EndAddr - addr));
					size -= i.EndAddr - addr;
					index += i.EndAddr - addr;
					addr = i.EndAddr;
				}
			}

			throw new IndexOutOfRangeException($"Invalid memory range 0x{originalAddr:X8} to 0x{(originalAddr + originalSize):X8}");
		}

		public byte[] ReadMem(uint addr, uint size)
		{
			var data = new byte[size];
			foreach (var entry in GetMemoryMapsForRange(addr, size))
			{
				var part = entry.Key.Read(entry.Value.startAddr, entry.Value.size);
				part.CopyTo(data, entry.Value.startIndex);
			}
			return data;
		}

		public byte ReadMem(uint addr) => ReadMem<byte>(addr);
		public T ReadMem<T>(uint addr) where T : IBinaryInteger<T> => T.ReadBigEndian(ReadMem(addr, TypeInfo<T>.Size), TypeInfo<T>.IsUnsigned);

		public void WriteMem(uint addr, byte[] data)
		{
			foreach (var entry in GetMemoryMapsForRange(addr, (uint)data.Length))
			{
				var part = new byte[entry.Value.size];
				Array.Copy(data, entry.Value.startIndex, part, 0, part.Length);
				entry.Key.Write(entry.Value.startAddr, part);
			}
		}

		public void WriteMem<T>(uint addr, T val) where T : IBinaryInteger<T>
		{
			var data = new byte[TypeInfo<T>.Size];
			val.WriteBigEndian(data);
			WriteMem(addr, data);
		}

		public void Run(bool blockThread = true)
		{
			Started = true;

			foreach (var i in Components)
			{
				i.Run();
			}

			if (blockThread)
			{
				foreach (var i in Components)
				{
					i.WaitUntilFinish();
				}
			}
		}

		public void Stop()
		{
			StoppingOrStopped = true;
			foreach (var i in Components)
			{
				i.Stop();
			}
		}

		public void PrintDebug(string text)
		{
			if (Debug) Console.WriteLine(text);
		}

		public void Dispose()
		{
			Stop();
			GC.SuppressFinalize(this);
		}
	}
}
