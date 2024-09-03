using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Pointer<T> : Argument<T> where T : INumber<T>
	{
		public override T Read(CPUCore core, byte type, Func<ushort> supply) => core.Computer.ReadMem<T>(GetAddress(core, type, supply));
		public override void Write(T value, CPUCore core, byte type, Func<ushort> supply) => core.Computer.WriteMem(GetAddress(core, type, supply), value);

		public static uint GetAddress(CPUCore core, byte type, Func<ushort> supply) => type switch
		{
			0b0010 => Utils.Merge(supply(), supply()),

			// We love it when C# pretends that uint is long
			0b0011 => (uint)(Utils.Merge(supply(), supply()) + (short)core.Registers[supply()]),
			0b0100 => (uint)(Utils.Merge(core.Registers[supply()], core.Registers[supply()]) + (short)supply()),
			0b0101 => (uint)(Utils.Merge(core.Registers[supply()], core.Registers[supply()]) + (short)core.Registers[supply()]),

			_ => throw new ArgumentException($"Invalid type {type} for pointer argument"),
		};
	}
}
