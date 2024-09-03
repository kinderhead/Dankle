using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Pointer<T>(CPUCore core, byte type, Func<ushort> supply) : Argument<T>(core, type, supply) where T : INumber<T>
	{
        public override T Read() => Core.Computer.ReadMem<T>(GetAddress());
		public override void Write(T value) => Core.Computer.WriteMem(GetAddress(), value);

		public uint GetAddress() => Type switch
		{
			0b0010 => Utils.Merge(Supply(), Supply()),

			// We love it when C# pretends that uint is long
			0b0011 => (uint)(Utils.Merge(Supply(), Supply()) + (short)Core.Registers[Supply()]),
			0b0100 => (uint)(Utils.Merge(Core.Registers[Supply()], Core.Registers[Supply()]) + (short)Supply()),
			0b0101 => (uint)(Utils.Merge(Core.Registers[Supply()], Core.Registers[Supply()]) + (short)Core.Registers[Supply()]),

			_ => throw new ArgumentException($"Invalid type {Type} for pointer argument"),
		};
	}
}
