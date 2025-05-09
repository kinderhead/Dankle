﻿using Dankle.Components.Arguments;
using Dankle.Components.Instructions;
using System;
using System.Buffers.Binary;
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

		public uint ProgramCounter { get => Utils.Merge(Registers[14], Registers[15]); set { Registers[14] = (ushort)(value >>> 16); Registers[15] = (ushort)value; } }
		public uint StackPointer { get => Utils.Merge(Registers[12], Registers[13]); set { Registers[12] = (ushort)(value >>> 16); Registers[13] = (ushort)value; LowestSP = Math.Min(value, LowestSP); } }

		public uint LowestSP { get; private set; } = uint.MaxValue;

		public bool ShouldStep = false;

		private int _overflow;
		private int _carry;
		private int _zero;
		private int _compare;
		private int _sign;

		private int _le;

		public bool Overflow { get => Interlocked.CompareExchange(ref _overflow, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _overflow, 1, 0) : Interlocked.CompareExchange(ref _overflow, 0, 1); } }
		public bool Carry { get => Interlocked.CompareExchange(ref _carry, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _carry, 1, 0) : Interlocked.CompareExchange(ref _carry, 0, 1); } }
		public bool Zero { get => Interlocked.CompareExchange(ref _zero, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _zero, 1, 0) : Interlocked.CompareExchange(ref _zero, 0, 1); } }
		public bool Compare { get => Interlocked.CompareExchange(ref _compare, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _compare, 1, 0) : Interlocked.CompareExchange(ref _compare, 0, 1); } }
		public bool Sign { get => Interlocked.CompareExchange(ref _sign, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _sign, 1, 0) : Interlocked.CompareExchange(ref _sign, 0, 1); } }
		
		public bool LittleEndianEmulation { get => Interlocked.CompareExchange(ref _le, 1, 1) == 1; set { var _ = value ? Interlocked.CompareExchange(ref _le, 1, 0) : Interlocked.CompareExchange(ref _le, 0, 1); } }

		public byte Flags
		{
			get
			{
				byte flags = 0;

				if (Overflow) flags |= 0b10000000;
				if (Carry) flags |= 0b01000000;
				if (Zero) flags |= 0b00100000;
				if (Compare) flags |= 0b00010000;
				if (Sign) flags |= 0b00001000;

				return flags;
			}
			set
			{
				Overflow = (value & 0b10000000) != 0;
				Carry = (value & 0b01000000) != 0;
				Zero = (value & 0b00100000) != 0;
				Compare = (value & 0b00010000) != 0;
				Sign = (value & 0b00001000) != 0;
			}
		}

		// In Megahertz
		public double Clockspeed { get; private set; }

		public CPUCore(Computer computer) : base(computer)
		{
			ALU = new(this);
			Registers = new(this);
			
			RegisterHandler((CPUStepMsg i) =>
			{
				Cycle();
				return !ShouldStop;
			});
		}

		public bool Step(Component? source = null) => Send<CPUStepMsg, bool>(new CPUStepMsg { Source = source });

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
			Computer.WriteMem(StackPointer, FixEndian(val));
		}

		public T Pop<T>() where T : IBinaryInteger<T>
		{
			T ret = FixEndian(Computer.ReadMem<T>(StackPointer));
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
			sb.Append($"PC: 0x{ProgramCounter:X8}\n");
			sb.Append($"Flags: 0b{Flags:B8}");

			return sb.ToString();
		}

		public void Dump() => Console.WriteLine(GetDump());

		public string Dissassemble(uint addr, bool resetPC = true)
		{
			var oldPC = ProgramCounter;
			ProgramCounter = addr;
			var insn = Instruction.Get(GetNext());
			var text = new StringBuilder(insn.Name.ToLower() + " ");

			int idex = 0;

			var ctx = Instruction.GetNextContext(this);
			foreach (var i in insn.Arguments)
			{
				var arg = IArgument.Create(i, ctx, idex++);

				text.Append(arg.Dissassemble());
				text.Append(", ");
			}

			if (idex > 0) text.Length -= 2;

			if (resetPC) ProgramCounter = oldPC;
			return text.ToString();
		}

		private void Cycle()
		{
			var sw = new Stopwatch();
			sw.Start();
			var addr = ProgramCounter;

			// Console.WriteLine(Dissassemble(addr));

			var op = GetNext();
			var insn = Instruction.Get(op);

			try
			{
				insn.Execute(this);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);

				var symbol = "";

				if (Computer.Symbols.Count > 0)
				{
					foreach (var i in Computer.Symbols.Reverse())
					{
						if (i.Value <= addr)
						{
							symbol = $" ({i.Key}+0x{addr - i.Value:X8})";
							break;
						}
					}
				}

				Console.Error.WriteLine($"Error at insn {insn.Name} at 0x{addr:X8}{symbol}\n\n{GetDump()}");

				Computer.StartDebugAsTask();
			}

			sw.Stop();
			Clockspeed = 1.0 / sw.Elapsed.TotalMicroseconds;
		}

		public T FixEndian<T>(T val) where T : IBinaryInteger<T>
		{
			if (LittleEndianEmulation)
			{
				if (val is ushort us) return (T)(object)BinaryPrimitives.ReverseEndianness(us);
				else if (val is short s) return (T)(object)BinaryPrimitives.ReverseEndianness(s);
				else if (val is uint ui) return (T)(object)BinaryPrimitives.ReverseEndianness(ui);
				else if (val is int i) return (T)(object)BinaryPrimitives.ReverseEndianness(i);
				else if (val is byte) return val;
				else throw new NotImplementedException();
			}
			return val;
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

			private readonly ushort[] registers = [.. Enumerable.Repeat((ushort)0, 16)];
			private readonly RegisterState[] states = [.. Enumerable.Repeat(RegisterState.None, 16)];

			public ushort this[int reg]
			{
				get
				{
					var val = registers[reg];
					return states[reg] switch
					{
						RegisterState.None => val,
						RegisterState.High => (ushort)(val >>> 8),
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
