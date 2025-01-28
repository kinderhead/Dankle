using Dankle.Components;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public readonly CPUCore MainCore;

		public SortedDictionary<string, uint> Symbols = [];

		public Computer(uint memSize)
		{
			MemorySize = memSize;
			AddMemoryMapEntry(new RAM(memSize));
			AddComponent<CPUCore>();
			MainCore = GetComponent<CPUCore>();
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

		public ReadOnlyCollection<Component> GetComponents() => Components.AsReadOnly();

		public void AddMemoryMapEntry(MemoryMapEntry entry)
		{
			MemoryMap.Add(entry);
			MemoryMap.Sort();
		}

		public void AddMemoryMapEntry(MemoryMapRegisters entry)
		{
			entry.Register(this);
		}

		public List<KeyValuePair<MemoryMapEntry, (uint startAddr, uint startIndex, uint size)>> GetMemoryMapsForRange(uint addr, uint size)
		{
			var entries = new List<KeyValuePair<MemoryMapEntry, (uint startAddr, uint startIndex, uint size)>>();

			uint originalAddr = addr;
			uint originalSize = size;
			uint index = 0;

			foreach (var i in MemoryMap)
			{
				if (addr >= i.StartAddr && addr < i.EndAddr)
				{
					if (addr + size <= i.EndAddr)
					{
						entries.Add(new(i, (addr, index, size)));
						return entries;
					}
					entries.Add(new(i, (addr, index, i.EndAddr - addr)));
					size -= i.EndAddr - addr;
					index += i.EndAddr - addr;
					addr = i.EndAddr;
				}
			}

			throw new IndexOutOfRangeException($"Invalid memory range 0x{originalAddr:X8} to 0x{originalAddr + originalSize - 1:X8}");
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
			//if (addr == 0x0000FFE3) StartDebugAsTask();

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

		public void RunDebug()
		{
			MainCore.ShouldStep = true;
			Debug = true;
			Run(false);

			StartDebugging();
		}

		public void StartDebugAsTask()
		{
			if (MainCore.ShouldStep) return;

			MainCore.ShouldStep = true;
			Task.Run(StartDebugging);
		}

		public void StartDebugging()
		{
			MainCore.ShouldStep = true;
			Debug = true;

			while (!StoppingOrStopped)
			{
				try
				{
					Console.Write("Dbg > ");
					var args = Console.ReadLine()?.Split(" ");
					if (args is null) break;

					var cmd = args[0];

					if (cmd == "") MainCore.Step();
					else if (cmd == "dump") MainCore.Dump();
					else if (cmd == "read")
					{
						var addr = uint.Parse(args[1], System.Globalization.NumberStyles.HexNumber);
						var val = ReadMem(addr);
						Console.WriteLine($"0x{addr:X8}: {val} | 0x{val:X2} | {Encoding.UTF8.GetString([val])}");
					}
					else if (cmd == "go") break;
					else if (cmd == "goto")
					{
						var addr = uint.Parse(args[1], System.Globalization.NumberStyles.HexNumber);
						while (MainCore.ProgramCounter != addr) MainCore.Step();
					}
					else if (cmd == "dis")
					{
						var pc = MainCore.ProgramCounter;
						for (var i = 0; i < int.Parse(args[1]); i++)
						{
							try
							{
								Console.WriteLine(MainCore.Dissassemble(MainCore.ProgramCounter, false));
							}
							catch
							{
								break;
							}
						}
						MainCore.ProgramCounter = pc;
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine(e.Message);
				}
			}

			MainCore.ShouldStep = false;
			Debug = false;
			MainCore.Step();
		}

		public void Dispose()
		{
			Stop();
			GC.SuppressFinalize(this);
		}
	}
}
