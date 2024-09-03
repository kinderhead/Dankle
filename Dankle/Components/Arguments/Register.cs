using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Register(CPUCore core, byte type, Func<ushort> supply) : Argument<ushort>(core, type, supply)
	{
        public override ushort Read() => Core.Registers[Type];
		public override void Write(ushort value) => Core.Registers[Type] = value;
	}
}
