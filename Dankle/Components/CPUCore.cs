using Dankle.Components.Instructions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class CPUCore : Component
	{
		public override string Name => "CPU Core";

		public readonly ConcurrentDictionary<int, ushort> Registers = new(Enumerable.Range(0, 16).ToDictionary(i => i, i => (ushort)0));

		public readonly ALU ALU;

		public uint ProgramCounter { get => Utils.Merge(Registers[14], Registers[15]); set { Registers[14] = (ushort)(value >> 16); Registers[15] = (ushort)value; } }
		public uint StackPointer { get => Utils.Merge(Registers[12], Registers[13]); set { Registers[12] = (ushort)(value >> 16); Registers[13] = (ushort)value; } }

		public bool ShouldStep = false;

		private int _overflow;
		private int _zero;
		private int _compare;

		public bool Overflow { get => Interlocked.CompareExchange(ref _overflow, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _overflow, 1, 0) : Interlocked.CompareExchange(ref _overflow, 0, 1); } }
		public bool Zero { get => Interlocked.CompareExchange(ref _zero, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _zero, 1, 0) : Interlocked.CompareExchange(ref _zero, 0, 1); } }
		public bool Compare { get => Interlocked.CompareExchange(ref _compare, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _compare, 1, 0) : Interlocked.CompareExchange(ref _compare, 0, 1); } }

		public CPUCore(Computer computer) : base(computer)
		{
			ALU = new(this);
			StackPointer = computer.MemorySize - 1;
			
			RegisterHandler((CPUStepMsg i) =>
			{
				Cycle();
				return !ShouldStop;
			});
		}

		public void Step(Component? source = null) => Send<CPUStepMsg, bool>(new CPUStepMsg { Source = source });

		protected override void Process()
		{
			while (!ShouldStop)
			{
				HandleMessage(ShouldStep);
				if (!ShouldStep) Cycle();
			}
		}

		public ushort GetNext() => GetNext<ushort>();
		public T GetNext<T>() where T : IBinaryInteger<T>
		{
			var val = Computer.ReadMem<T>(ProgramCounter);
			ProgramCounter += (ushort)TypeInfo<T>.Size;
			return val;
		}

		public void Push<T>(T val) where T : IBinaryInteger<T>
		{
			StackPointer -= TypeInfo<T>.Size;
			Computer.WriteMem(ProgramCounter, val);
		}

		public T Pop<T>() where T : IBinaryInteger<T>
		{
			T ret = Computer.ReadMem<T>(StackPointer);
			StackPointer += TypeInfo<T>.Size;
			return ret;
		}

		private void Cycle()
		{
			var op = GetNext();
			Instruction.Get(op).Execute(this);
		}
	}

	public class CPUStepMsg : Message<bool>
	{

	}
}
