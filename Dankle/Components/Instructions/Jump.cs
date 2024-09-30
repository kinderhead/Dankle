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

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "JMP";

		protected override void Handle(Context ctx)
		{
			ctx.Core.ProgramCounter = ctx.GetNextArg<Any32>().Read();
		}
	}

	public class JumpEq : Instruction
	{
		public override ushort Opcode => 21;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "JE";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>().Read();
			if (ctx.Core.Compare) ctx.Core.ProgramCounter = dest;
		}
	}

	public class JumpNeq : Instruction
	{
		public override ushort Opcode => 22;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "JNE";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>().Read();
			if (!ctx.Core.Compare) ctx.Core.ProgramCounter = dest;
		}
	}
}
