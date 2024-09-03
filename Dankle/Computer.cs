using Dankle.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle
{
	public class Computer : IDisposable
	{
		private readonly List<Component> Components = [];

		private readonly byte[] Memory;
		private readonly Lock MemoryLock = new();

		public Computer(int memSize)
		{
			Memory = new byte[memSize];
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

		public byte ReadMem(uint addr)
		{
			lock (MemoryLock) return Memory[addr];
		}

		public ushort ReadMem16(uint addr)
		{
			lock (MemoryLock) return Utils.Merge(Memory[addr + 1], Memory[addr]);
		}

		public T ReadMem<T>(uint addr) where T : INumber<T>
		{
			if (typeof(T) == typeof(byte)) return (T)(INumber<byte>)ReadMem(addr);
			else if (typeof(T) == typeof(ushort)) return (T)(INumber<ushort>)ReadMem16(addr);
			throw new ArgumentException($"Invalid type {typeof(T).Name}");
		}

		public void WriteMem<T>(uint addr, T val) where T : INumber<T>
		{
			if (val is byte b) lock (MemoryLock) Memory[addr] = b;
			else if (val is ushort s)
			{
				lock (MemoryLock)
				{
					Memory[addr] = (byte)(s >> 8);
					Memory[addr + 1] = (byte)s;
				}
			}
			else throw new ArgumentException($"Invalid type {typeof(T).Name}");
		}

		public void Run(bool blockThread = true)
		{
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
			foreach (var i in Components)
			{
				i.Stop();
			}
		}

		public void Dispose()
		{
			Stop();
			GC.SuppressFinalize(this);
		}
	}
}
