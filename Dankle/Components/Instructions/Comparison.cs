using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

			ctx.Core.ALU.CompareAndSetFlag((short)arg1.Read(), Comparison.EQ, (short)arg2.Read());
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

			ctx.Core.ALU.CompareAndSetFlag((short)arg1.Read(), Comparison.LT, (short)arg2.Read());
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

			ctx.Core.ALU.CompareAndSetFlag((short)arg1.Read(), Comparison.LTE, (short)arg2.Read());
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

			ctx.Core.ALU.CompareAndSetFlag((short)arg1.Read(), Comparison.GT, (short)arg2.Read());
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

			ctx.Core.ALU.CompareAndSetFlag((short)arg1.Read(), Comparison.GTE, (short)arg2.Read());
		}
	}

	public class CompareFlags : Instruction
	{
		public override ushort Opcode => 42;

		public override Type[] Arguments => [typeof(Any8Num), typeof(Any8Num)];

		public override string Name => "CMPF";

		protected override void Handle(Context ctx)
		{
			var mask = ctx.GetNextArg<Any8Num>();
			var check = ctx.GetNextArg<Any8Num>();

			ctx.Core.Compare = (ctx.Core.Flags & mask.Read()) == check.Read();
		}
	}

	public class CompareFlagsEven : Instruction
	{
		public override ushort Opcode => 43;

		public override Type[] Arguments => [typeof(Any8Num)];

		public override string Name => "CMPFE";

		protected override void Handle(Context ctx)
		{
			var mask = ctx.GetNextArg<Any8Num>();

			ctx.Core.Compare = BitOperations.PopCount((uint)(ctx.Core.Flags & mask.Read())) % 2 == 0;
		}
	}

	public class CompareFlagsOdd : Instruction
	{
		public override ushort Opcode => 44;

		public override Type[] Arguments => [typeof(Any8Num)];

		public override string Name => "CMPFO";

		protected override void Handle(Context ctx)
		{
			var mask = ctx.GetNextArg<Any8Num>();

			ctx.Core.Compare = BitOperations.PopCount((uint)(ctx.Core.Flags & mask.Read())) % 2 == 1;
		}
	}
}
