using Dankle.Components.Instructions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class CPUCore : Component
	{
		public override string Name => "CPU Core";

		public readonly RegisterHandler Registers;

		public readonly ALU ALU;

		public uint ProgramCounter { get => Utils.Merge(Registers[14], Registers[15]); set { Registers[14] = (ushort)(value >> 16); Registers[15] = (ushort)value; } }
		public uint StackPointer { get => Utils.Merge(Registers[12], Registers[13]); set { Registers[12] = (ushort)(value >> 16); Registers[13] = (ushort)value; } }

		public bool ShouldStep = false;

		private int _overflow;
		private int _carry;
		private int _zero;
		private int _compare;

		public bool Overflow { get => Interlocked.CompareExchange(ref _overflow, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _overflow, 1, 0) : Interlocked.CompareExchange(ref _overflow, 0, 1); } }
		public bool Carry { get => Interlocked.CompareExchange(ref _carry, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _carry, 1, 0) : Interlocked.CompareExchange(ref _carry, 0, 1); } }
		public bool Zero { get => Interlocked.CompareExchange(ref _zero, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _zero, 1, 0) : Interlocked.CompareExchange(ref _zero, 0, 1); } }
		public bool Compare { get => Interlocked.CompareExchange(ref _compare, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _compare, 1, 0) : Interlocked.CompareExchange(ref _compare, 0, 1); } }

		// In Megahertz
		public double Clockspeed { get; private set; }

		public CPUCore(Computer computer) : base(computer)
		{
			ALU = new(this);
			Registers = new(this);
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
			Computer.WriteMem(StackPointer, val);
		}

		public T Pop<T>() where T : IBinaryInteger<T>
		{
			T ret = Computer.ReadMem<T>(StackPointer);
			StackPointer += TypeInfo<T>.Size;
			return ret;
		}

		public string GetDump()
		{
			var sb = new StringBuilder();

			int idex = 0;
			foreach (var i in Registers)
			{
				sb.Append($"r{idex++}: 0x{i:X4}\n");
			}

			sb.Append($"SP: 0x{StackPointer:X8}\n");
			sb.Append($"PC: 0x{ProgramCounter:X8}");

			return sb.ToString();
		}

		public void Dump() => Console.WriteLine(GetDump());

		private void Cycle()
		{
			var sw = new Stopwatch();
			sw.Start();
			var op = GetNext();
			Instruction.Get(op).Execute(this);
			sw.Stop();
			Clockspeed = 1.0 / sw.Elapsed.TotalMicroseconds;
		}

		public class RegisterHandler(CPUCore core) : IEnumerable<ushort>
		{
			public enum RegisterState
			{
				None,
				High,
				Low
			}

			public readonly CPUCore Core = core;

			private readonly ushort[] registers = Enumerable.Repeat((ushort)0, 16).ToArray();
			private readonly RegisterState[] states = Enumerable.Repeat(RegisterState.None, 16).ToArray();

			public ushort this[int reg]
			{
				get
				{
					var val = registers[reg];
					return states[reg] switch
					{
						RegisterState.None => val,
						RegisterState.High => (ushort)(val >> 8),
						RegisterState.Low => (ushort)(val & 0xFF),
						_ => throw new Exception("Invalid register state somehow"),
					};
				}
				set
				{
					registers[reg] = states[reg] switch
					{
						RegisterState.None => value,
						RegisterState.High => (ushort)((registers[reg] & 0x00FF) | (value << 8)),
						RegisterState.Low => (ushort)((registers[reg] & 0xFF00) | value),
						_ => throw new Exception("Invalid register state somehow"),
					};
				}
			}

			public void SetState(int reg, RegisterState state) => states[reg] = state;

			public IEnumerator<ushort> GetEnumerator() => ((IEnumerable<ushort>)registers).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => registers.GetEnumerator();
		}
	}

	public class CPUStepMsg : Message<bool>
	{

	}
}
