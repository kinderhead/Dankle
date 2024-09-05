using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Load : Instruction
	{
		public override ushort Opcode => 2;

		protected override void Handle(Context ctx)
		{
			var dest = ctx.Arg<Register>();
			var src = ctx.Arg<Any16Num>();

			dest.Write(src.Read());
		}
	}
}
