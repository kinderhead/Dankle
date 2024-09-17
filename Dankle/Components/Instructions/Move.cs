using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Move : Instruction
	{
		public override ushort Opcode => 6;

		public override Type[] Arguments => [typeof(Register), typeof(Register)];
		public override string Name => "MOV";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Register>();
			var src = ctx.GetNextArg<Register>();

			dest.Write(src.Read());
		}
	}
}
