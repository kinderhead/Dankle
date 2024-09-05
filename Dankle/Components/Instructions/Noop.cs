using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Noop : Instruction
	{
		public override ushort Opcode => 1;

		protected override void Handle(Context ctx)
		{
			
		}
	}
}
