﻿using Dankle.Components.Arguments;
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

		public override Type[] Arguments => [typeof(Register), typeof(Any16Num)];
		public override string Name => "LD";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Register>();
			var src = ctx.GetNextArg<Any16Num>();

			dest.Write(src.Read());
		}
	}

	public class Load8 : Instruction
	{
		public override ushort Opcode => 4;

		public override Type[] Arguments => [typeof(Register), typeof(Any8Num)];
		public override string Name => "LDB";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Register>();
			var src = ctx.GetNextArg<Any8Num>();

			dest.Write(src.Read());
		}
	}
}
