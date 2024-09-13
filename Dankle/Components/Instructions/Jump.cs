using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Jump : Instruction
	{
		public override ushort Opcode => 20;

		protected override void Handle(Context ctx)
		{
			ctx.Core.ProgramCounter = ctx.GetNextArg<Any32>().Read();
		}
	}

	public class JumpEq : Instruction
	{
		public override ushort Opcode => 21;

		protected override void Handle(Context ctx)
		{
			if (ctx.Core.Compare) ctx.Core.ProgramCounter = ctx.GetNextArg<Any32>().Read();
		}
	}

	public class JumpNeq : Instruction
	{
		public override ushort Opcode => 22;

		protected override void Handle(Context ctx)
		{
			if (!ctx.Core.Compare) ctx.Core.ProgramCounter = ctx.GetNextArg<Any32>().Read();
		}
	}
}
