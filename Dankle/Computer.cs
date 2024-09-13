using Dankle.Components;
using System;
using System.Collections.Generic;
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

		private readonly byte[] Memory;
		private readonly Lock MemoryLock = new();

		public readonly uint MemorySize;

		public bool StoppingOrStopped { get; private set; }

		public Computer(uint memSize)
		{
			MemorySize = memSize;
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

		public byte[] ReadMem(uint addr, uint size)
		{
			var data = new byte[size];
			lock (MemoryLock)
			{
				Array.Copy(Memory, addr, data, 0, size);
			}
			return data;
		}

		public byte ReadMem(uint addr) => ReadMem<byte>(addr);
		public T ReadMem<T>(uint addr) where T : IBinaryInteger<T> => T.ReadBigEndian(ReadMem(addr, TypeInfo<T>.Size), TypeInfo<T>.IsUnsigned);

		public void WriteMem(uint addr, byte[] data)
		{
			lock (MemoryLock)
			{
				data.CopyTo(Memory, addr);
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

		public void Dispose()
		{
			Stop();
			GC.SuppressFinalize(this);
		}
	}
}
