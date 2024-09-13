﻿using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Call : Instruction
	{
		public override ushort Opcode => 27;

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>();

			ctx.Core.Push(ctx.Core.ProgramCounter);
			ctx.Core.ProgramCounter = dest.Read();
		}
	}

	public class Return : Instruction
	{
		public override ushort Opcode => 28;

		protected override void Handle(Context ctx)
		{
			ctx.Core.ProgramCounter = ctx.Core.Pop<uint>();
		}
	}

	public class Push : Instruction
	{
		public override ushort Opcode => 29;

		protected override void Handle(Context ctx)
		{
			var arg = ctx.GetNextArg<Any16>();
			ctx.Core.Push(arg.Read());
		}
	}

	public class Pop : Instruction
	{
		public override ushort Opcode => 30;

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any16>();
			dest.Write(ctx.Core.Pop<ushort>());
		}
	}
}
