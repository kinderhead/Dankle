using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Immediate : Argument<ushort>
	{
		public override ushort Read(CPUCore core, byte type, Func<ushort> supply, ushort[] registers) => supply();
		public override void Write(ushort value, CPUCore core, byte type, Func<ushort> supply, ushort[] registers) => throw new InvalidOperationException("Cannot write to an immediate value");
	}
}
