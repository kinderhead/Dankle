using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class LE : Instruction
	{
		public override ushort Opcode => 49;

		public override Type[] Arguments => [];

		public override string Name => "LE";

		protected override void Handle(Context ctx)
		{
			ctx.Core.LittleEndianEmulation = true;
		}
	}

	public class BE : Instruction
	{
		public override ushort Opcode => 50;

		public override Type[] Arguments => [];

		public override string Name => "BE";

		protected override void Handle(Context ctx)
		{
			ctx.Core.LittleEndianEmulation = false;
		}
	}
}
