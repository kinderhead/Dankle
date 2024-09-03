using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public interface IArgument
	{

	}

	public abstract class Argument<T>(CPUCore core, byte type, Func<ushort> supply) : IArgument where T : INumber<T>
	{
		public CPUCore Core = core;
		public byte Type = type;
		public Func<ushort> Supply = supply;

		public abstract T Read();
		public abstract void Write(T value);
	}
}
