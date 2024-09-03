using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Immediate(CPUCore core, byte type, Func<ushort> supply) : Argument<ushort>(core, type, supply)
	{
        public override ushort Read() => Supply();
		public override void Write(ushort value) => throw new InvalidOperationException("Cannot write to an immediate value");
	}
}
