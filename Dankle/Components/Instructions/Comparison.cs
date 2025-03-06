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

	public class UnsignedLessThan : Instruction
	{
		public override ushort Opcode => 63;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "ULT";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.LT, arg2.Read());
		}
	}

	public class UnsignedLessThanOrEq : Instruction
	{
		public override ushort Opcode => 64;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "ULTE";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.LTE, arg2.Read());
		}
	}

	public class UnsignedGreaterThan : Instruction
	{
		public override ushort Opcode => 65;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "UGT";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.GT, arg2.Read());
		}
	}

	public class UnsignedGreaterThanOrEq : Instruction
	{
		public override ushort Opcode => 66;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "UGTE";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.GTE, arg2.Read());
		}
	}

	public class LessThan32 : Instruction
	{
		public override ushort Opcode => 67;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "LTL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();

			ctx.Core.ALU.CompareAndSetFlag((int)arg1.Read(), Comparison.LT, (int)arg2.Read());
		}
	}

	public class LessThanOrEq32 : Instruction
	{
		public override ushort Opcode => 68;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "LTEL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();

			ctx.Core.ALU.CompareAndSetFlag((int)arg1.Read(), Comparison.LTE, (int)arg2.Read());
		}
	}

	public class GreaterThan32 : Instruction
	{
		public override ushort Opcode => 69;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "GTL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();

			ctx.Core.ALU.CompareAndSetFlag((int)arg1.Read(), Comparison.GT, (int)arg2.Read());
		}
	}

	public class GreaterThanOrEq32 : Instruction
	{
		public override ushort Opcode => 70;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "GTEL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();

			ctx.Core.ALU.CompareAndSetFlag((int)arg1.Read(), Comparison.GTE, (int)arg2.Read());
		}
	}

	public class UnsignedLessThan32 : Instruction
	{
		public override ushort Opcode => 71;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "ULTL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.LT, arg2.Read());
		}
	}

	public class UnsignedLessThanOrEq32 : Instruction
	{
		public override ushort Opcode => 72;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "ULTEL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.LTE, arg2.Read());
		}
	}

	public class UnsignedGreaterThan32 : Instruction
	{
		public override ushort Opcode => 73;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "UGTL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.GT, arg2.Read());
		}
	}

	public class UnsignedGreaterThanOrEq32 : Instruction
	{
		public override ushort Opcode => 74;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "UGTEL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.GTE, arg2.Read());
		}
	}

	public class LessThan64 : Instruction
	{
		public override ushort Opcode => 80;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "LTLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();

			ctx.Core.ALU.CompareAndSetFlag((long)arg1.Read(), Comparison.LT, (long)arg2.Read());
		}
	}

	public class LessThanOrEq64 : Instruction
	{
		public override ushort Opcode => 81;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "LTELL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();

			ctx.Core.ALU.CompareAndSetFlag((long)arg1.Read(), Comparison.LTE, (long)arg2.Read());
		}
	}

	public class GreaterThan64 : Instruction
	{
		public override ushort Opcode => 82;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "GTLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();

			ctx.Core.ALU.CompareAndSetFlag((long)arg1.Read(), Comparison.GT, (long)arg2.Read());
		}
	}

	public class GreaterThanOrEq64 : Instruction
	{
		public override ushort Opcode => 83;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "GTELL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();

			ctx.Core.ALU.CompareAndSetFlag((long)arg1.Read(), Comparison.GTE, (long)arg2.Read());
		}
	}

	public class UnsignedLessThan64 : Instruction
	{
		public override ushort Opcode => 84;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "ULTLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.LT, arg2.Read());
		}
	}

	public class UnsignedLessThanOrEq64 : Instruction
	{
		public override ushort Opcode => 85;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "ULTELL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.LTE, arg2.Read());
		}
	}

	public class UnsignedGreaterThan64 : Instruction
	{
		public override ushort Opcode => 86;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "UGTLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.GT, arg2.Read());
		}
	}

	public class UnsignedGreaterThanOrEq64 : Instruction
	{
		public override ushort Opcode => 87;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "UGTELL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();

			ctx.Core.ALU.CompareAndSetFlag(arg1.Read(), Comparison.GTE, arg2.Read());
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

	public class FlipCompare : Instruction
	{
		public override ushort Opcode => 75;

		public override Type[] Arguments => [];

		public override string Name => "FCMP";

		protected override void Handle(Context ctx)
		{
			ctx.Core.Compare = !ctx.Core.Compare;
		}
	}
}
