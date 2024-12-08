using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Call : Instruction
	{
		public override ushort Opcode => 29;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "CALL";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>().Read();

			ctx.Core.Push(ctx.Core.ProgramCounter);
			ctx.Core.ProgramCounter = dest;
		}
	}

	public class XCall : Instruction
	{
		public override ushort Opcode => 52;

		public override Type[] Arguments => [typeof(Any32)];
		public override string Name => "XCALL";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any32>().Read();

			ctx.Core.Push(ctx.Core.Registers[15]);
			ctx.Core.ProgramCounter = dest;
		}
	}

	public class Return : Instruction
	{
		public override ushort Opcode => 30;

		public override Type[] Arguments => [];
		public override string Name => "RET";

		protected override void Handle(Context ctx)
		{
			ctx.Core.ProgramCounter = ctx.Core.Pop<uint>();
		}
	}

	public class Push : Instruction
	{
		public override ushort Opcode => 31;

		public override Type[] Arguments => [typeof(Any16)];
		public override string Name => "PUSH";

		protected override void Handle(Context ctx)
		{
			var arg = ctx.GetNextArg<Any16>();
			ctx.Core.Push(arg.Read());
		}
	}

	public class Pop : Instruction
	{
		public override ushort Opcode => 32;

		public override Type[] Arguments => [typeof(Any16)];
		public override string Name => "POP";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any16>();
			dest.Write(ctx.Core.Pop<ushort>());
		}
	}

	public class PushFlags : Instruction
	{
		public override ushort Opcode => 45;

		public override Type[] Arguments => [];
		public override string Name => "PUSHF";

		protected override void Handle(Context ctx)
		{
			ctx.Core.Push(ctx.Core.Flags);
		}
	}

	public class PopFlags : Instruction
	{
		public override ushort Opcode => 46;

		public override Type[] Arguments => [];
		public override string Name => "POPF";

		protected override void Handle(Context ctx)
		{
			ctx.Core.Flags = ctx.Core.Pop<byte>();
		}
	}

	public class PushRegisters : Instruction
	{
		public override ushort Opcode => 53;

		public override Type[] Arguments => [typeof(Any16)];
		public override string Name => "PUSHR";

		protected override void Handle(Context ctx)
		{
			var mask = ctx.GetNextArg<Any16>().Read();
			for (int i = 0; i < 16; i++)
			{
				if (((mask >>> i) & 1) == 1) ctx.Core.Push(ctx.Core.Registers[15 - i]);
			}
		}
	}

	public class PopRegisters : Instruction
	{
		public override ushort Opcode => 54;

		public override Type[] Arguments => [typeof(Any16)];
		public override string Name => "POPR";

		protected override void Handle(Context ctx)
		{
			var mask = ctx.GetNextArg<Any16>().Read();
			for (int i = 0; i < 16; i++)
			{
				if (((mask << i) & 0b1000000000000000) != 0) ctx.Core.Registers[i] = ctx.Core.Pop<ushort>();
			}
		}
	}
}
