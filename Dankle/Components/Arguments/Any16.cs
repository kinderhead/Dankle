using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Any16(CPUCore core, byte type, Func<ushort> supply) : Argument<ushort>(core, type, supply)
	{
        public override ushort Read() => GetArg(Type).Read();
		public override void Write(ushort value) => GetArg(Type).Write(value);

		public Argument<ushort> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate(Core, Type, Supply),
			0b0001 => new Register(Core, Type, Supply),
			0b0010 or 0b0011 or 0b0100 or 0b0101 => new Pointer<ushort>(Core, Type, Supply),
			_ => throw new ArgumentException($"Invalid type {type} for 16 bit any argument"),
		};
	}
}
