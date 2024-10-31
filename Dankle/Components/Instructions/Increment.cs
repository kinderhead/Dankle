using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Increment : Instruction
	{
		public override ushort Opcode => 31;

		public override Type[] Arguments => [typeof(Register)];

		public override string Name => "INC";

		protected override void Handle(Context ctx)
		{
			ctx.Core.Registers[ctx.Data[0]]++;
		}
	}

	public class Decrement : Instruction
	{
		public override ushort Opcode => 32;

		public override Type[] Arguments => [typeof(Register)];

		public override string Name => "DEC";

		protected override void Handle(Context ctx)
		{
			ctx.Core.Registers[ctx.Data[0]]--;
		}
	}
}
