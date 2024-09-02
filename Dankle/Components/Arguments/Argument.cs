using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public abstract class Argument<T>
	{
		public abstract T Read(CPUCore core, byte type, Func<ushort> supply, ushort[] registers);
		public abstract void Write(T value, CPUCore core, byte type, Func<ushort> supply, ushort[] registers);
	}
}
