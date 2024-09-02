using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Register : Argument<ushort>
	{
		public override ushort Read(CPUCore core, byte type, Func<ushort> supply, ushort[] registers) => registers[type];
		public override void Write(ushort value, CPUCore core, byte type, Func<ushort> supply, ushort[] registers) => registers[type] = value;
	}
}
