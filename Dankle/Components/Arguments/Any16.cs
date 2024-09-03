using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Any16 : Argument<ushort>
	{
		public override ushort Read(CPUCore core, byte type, Func<ushort> supply) => GetArg(type).Read(core, type, supply);
		public override void Write(ushort value, CPUCore core, byte type, Func<ushort> supply) => GetArg(type).Write(value, core, type, supply);

		public static Argument<ushort> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate(),
			0b0001 => new Register(),
			0b0010 or 0b0011 or 0b0100 or 0b0101 => new Pointer<ushort>(),
			_ => throw new ArgumentException($"Invalid type {type} for 16 bit any argument"),
		};
	}
}
