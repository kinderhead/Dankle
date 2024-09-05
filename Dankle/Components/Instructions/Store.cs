using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Store : Instruction
	{
		public override ushort Opcode => 3;

		protected override void Handle(Context ctx)
		{
			var dest = ctx.Arg<Any16Num>();
			var src = ctx.Arg<Register>();

			dest.Write(src.Read());
		}
	}
}
