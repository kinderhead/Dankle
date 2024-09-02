using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Register : Argument
	{
		public byte Index;

		public override void Build(byte type, Func<ushort> supply)
		{
			Index = type;
		}
	}
}
