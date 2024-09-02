using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public abstract class Argument
	{
		public abstract void Build(byte type, Func<ushort> supply);
	}
}
