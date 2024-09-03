using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public abstract class Argument<T> where T : INumber<T>
	{
		public abstract T Read(CPUCore core, byte type, Func<ushort> supply);
		public abstract void Write(T value, CPUCore core, byte type, Func<ushort> supply);
	}
}
