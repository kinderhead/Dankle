using Dankle.Components.Arguments;
using Dankle.Components.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Jump : Instruction, IJumpInsn
	{
		public override ushort Opcode => 20;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "JMP";

        public int JumpArgIndex => 0;

        protected override void Handle(Context ctx)
		{
			ctx.Core.ProgramCounter = ctx.GetNextArg<Any32>().Read();
		}
	}

	public class JumpEq : Instruction, IJumpInsn
	{
		public override ushort Opcode => 21;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "JE";

		public int JumpArgIndex => 0;

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>().Read();
			if (ctx.Core.Compare) ctx.Core.ProgramCounter = dest;
		}
	}

	public class JumpNeq : Instruction, IJumpInsn
	{
		public override ushort Opcode => 22;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "JNE";

		public int JumpArgIndex => 0;

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>().Read();
			if (!ctx.Core.Compare) ctx.Core.ProgramCounter = dest;
		}
	}

	public class JumpZ : Instruction, IJumpInsn
	{
		public override ushort Opcode => 23;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "JZ";

		public int JumpArgIndex => 0;

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>().Read();
			if (ctx.Core.Zero) ctx.Core.ProgramCounter = dest;
		}
	}

	public class JumpNZ : Instruction, IJumpInsn
	{
		public override ushort Opcode => 24;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "JNZ";

		public int JumpArgIndex => 0;

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>().Read();
			if (!ctx.Core.Zero) ctx.Core.ProgramCounter = dest;
		}
	}
}
