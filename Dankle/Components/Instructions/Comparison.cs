using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Compare : Instruction
	{
		public override ushort Opcode => 15;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "CMP";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.EQ, arg2.Read());
		}
	}

	public class LessThan : Instruction
	{
		public override ushort Opcode => 16;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "LT";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.LT, arg2.Read());
		}
	}

	public class LessThanOrEq : Instruction
	{
		public override ushort Opcode => 17;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "LTE";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.LTE, arg2.Read());
		}
	}

	public class GreaterThan : Instruction
	{
		public override ushort Opcode => 18;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "GT";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.GT, arg2.Read());
		}
	}

	public class GreaterThanOrEq : Instruction
	{
		public override ushort Opcode => 19;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "GTE";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.GTE, arg2.Read());
		}
	}
}
