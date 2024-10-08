﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Halt : Instruction
	{
		public override ushort Opcode => 0;

		public override Type[] Arguments => [];
		public override string Name => "HLT";

		protected override void Handle(Context ctx)
		{
			ctx.Core.ShouldStep = true;
			Task.Run(ctx.Core.Computer.Stop);
		}
	}
}
