using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Add : Instruction
	{
		public override ushort Opcode => 7;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "ADD";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.ADD, arg2.Read()));
		}
	}

	public class Adc : Instruction
	{
		public override ushort Opcode => 35;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "ADC";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			var carry = ctx.Core.Carry;

			var val = ctx.Core.ALU.Calculate(arg1.Read(), Operation.ADD, arg2.Read());
			var didCarry = ctx.Core.Carry;
			if (carry) val = ctx.Core.ALU.Calculate(val, Operation.ADD, (ushort)1);
			ctx.Core.Carry = ctx.Core.Carry || didCarry;
			dest.Write(val);
		}
	}

	public class Subtract : Instruction
	{
		public override ushort Opcode => 8;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "SUB";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.SUB, arg2.Read()));
		}
	}

	public class Sbb : Instruction
	{
		public override ushort Opcode => 39;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "SBB";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			var carry = ctx.Core.Carry;

			var val = ctx.Core.ALU.Calculate(arg1.Read(), Operation.SUB, arg2.Read());
			var didCarry = ctx.Core.Carry;
			if (carry) val = ctx.Core.ALU.Calculate(val, Operation.SUB, (ushort)1);
			ctx.Core.Carry = ctx.Core.Carry || didCarry;
			dest.Write(val);
		}
	}

	public class SignedMul : Instruction
	{
		public override ushort Opcode => 9;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "SMUL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write((ushort)ctx.Core.ALU.Calculate((short)arg1.Read(), Operation.MUL, (short)arg2.Read()));
		}
	}

	public class UnsignedMul : Instruction
	{
		public override ushort Opcode => 11;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "UMUL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.MUL, arg2.Read()));
		}
	}

	public class UnsignedMul32 : Instruction
	{
		public override ushort Opcode => 59;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32), typeof(Any32)];
		public override string Name => "UMULL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.MUL, arg2.Read()));
		}
	}

	public class SignedMul32 : Instruction
	{
		public override ushort Opcode => 58;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32), typeof(Any32)];
		public override string Name => "SMULL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write((uint)ctx.Core.ALU.Calculate((int)arg1.Read(), Operation.MUL, (int)arg2.Read()));
		}
	}

	public class UnsignedMul64 : Instruction
	{
		public override ushort Opcode => 77;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64), typeof(Any64)];
		public override string Name => "UMULLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.MUL, arg2.Read()));
		}
	}

	public class SignedMul64 : Instruction
	{
		public override ushort Opcode => 76;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64), typeof(Any64)];
		public override string Name => "SMULLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write((ulong)ctx.Core.ALU.Calculate((long)arg1.Read(), Operation.MUL, (long)arg2.Read()));
		}
	}

	public class SignedDiv : Instruction
	{
		public override ushort Opcode => 10;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "SDIV";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write((ushort)ctx.Core.ALU.Calculate((short)arg1.Read(), Operation.DIV, (short)arg2.Read()));
		}
	}

	public class UnsignedDiv : Instruction
	{
		public override ushort Opcode => 12;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "UDIV";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.DIV, arg2.Read()));
		}
	}

	public class UnsignedDiv32 : Instruction
	{
		public override ushort Opcode => 40;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32), typeof(Any32)];
		public override string Name => "UDIVL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.DIV, arg2.Read()));
		}
	}

	public class SignedDiv64 : Instruction
	{
		public override ushort Opcode => 78;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64), typeof(Any64)];
		public override string Name => "SDIVLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write((ulong)ctx.Core.ALU.Calculate((long)arg1.Read(), Operation.DIV, (long)arg2.Read()));
		}
	}

	public class UnsignedDiv64 : Instruction
	{
		public override ushort Opcode => 79;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64), typeof(Any64)];
		public override string Name => "UDIVLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.DIV, arg2.Read()));
		}
	}

	public class SignedDiv32 : Instruction
	{
		public override ushort Opcode => 47;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32), typeof(Any32)];
		public override string Name => "SDIVL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write((uint)ctx.Core.ALU.Calculate((int)arg1.Read(), Operation.DIV, (int)arg2.Read()));
		}
	}

	public class LeftShift : Instruction
	{
		public override ushort Opcode => 13;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "LSH";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Shift(arg1.Read(), ShiftOperation.LSH, arg2.Read()));
		}
	}

	public class LeftShift32 : Instruction
	{
		public override ushort Opcode => 95;

		public override Type[] Arguments => [typeof(Any32), typeof(Any16), typeof(Any32)];
		public override string Name => "LSHL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write(ctx.Core.ALU.Shift(arg1.Read(), ShiftOperation.LSH, arg2.Read()));
		}
	}

	public class LeftShift64 : Instruction
	{
		public override ushort Opcode => 96;

		public override Type[] Arguments => [typeof(Any64), typeof(Any16), typeof(Any64)];
		public override string Name => "LSHLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write(ctx.Core.ALU.Shift(arg1.Read(), ShiftOperation.LSH, arg2.Read()));
		}
	}

	public class RightShift : Instruction
	{
		public override ushort Opcode => 14;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "RSH";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Shift(arg1.Read(), ShiftOperation.RSH, arg2.Read()));
		}
	}

	public class RightShift32 : Instruction
	{
		public override ushort Opcode => 97;

		public override Type[] Arguments => [typeof(Any32), typeof(Any16), typeof(Any32)];
		public override string Name => "RSHL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write(ctx.Core.ALU.Shift(arg1.Read(), ShiftOperation.RSH, arg2.Read()));
		}
	}

	public class RightShift64 : Instruction
	{
		public override ushort Opcode => 98;

		public override Type[] Arguments => [typeof(Any64), typeof(Any16), typeof(Any64)];
		public override string Name => "RSHLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write(ctx.Core.ALU.Shift(arg1.Read(), ShiftOperation.RSH, arg2.Read()));
		}
	}

	public class Or : Instruction
	{
		public override ushort Opcode => 25;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "OR";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Bitwise(arg1.Read(), BitwiseOperation.OR, arg2.Read()));
		}
	}

	public class And : Instruction
	{
		public override ushort Opcode => 26;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "AND";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Bitwise(arg1.Read(), BitwiseOperation.AND, arg2.Read()));
		}
	}

	public class Xor : Instruction
	{
		public override ushort Opcode => 27;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "XOR";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Bitwise(arg1.Read(), BitwiseOperation.XOR, arg2.Read()));
		}
	}

	public class Modulo : Instruction
	{
		public override ushort Opcode => 28;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "MOD";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.MOD, arg2.Read()));
		}
	}

	public class SignedModulo : Instruction
	{
		public override ushort Opcode => 92;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16), typeof(Any16)];
		public override string Name => "SMOD";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any16>();
			var arg2 = ctx.GetNextArg<Any16>();
			var dest = ctx.GetNextArg<Any16>();

			dest.Write((ushort)ctx.Core.ALU.Calculate((short)arg1.Read(), Operation.MOD, (short)arg2.Read()));
		}
	}

	public class Modulo32 : Instruction
	{
		public override ushort Opcode => 41;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32), typeof(Any32)];
		public override string Name => "UMODL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.MOD, arg2.Read()));
		}
	}

	public class SignedModulo32 : Instruction
	{
		public override ushort Opcode => 48;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32), typeof(Any32)];
		public override string Name => "SMODL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any32>();
			var arg2 = ctx.GetNextArg<Any32>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write((uint)ctx.Core.ALU.Calculate((int)arg1.Read(), Operation.MOD, (int)arg2.Read()));
		}
	}

	public class Modulo64 : Instruction
	{
		public override ushort Opcode => 94;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64), typeof(Any64)];
		public override string Name => "UMODLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.MOD, arg2.Read()));
		}
	}

	public class SignedModulo64 : Instruction
	{
		public override ushort Opcode => 93;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64), typeof(Any64)];
		public override string Name => "SMODLL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Any64>();
			var arg2 = ctx.GetNextArg<Any64>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write((ulong)ctx.Core.ALU.Calculate((long)arg1.Read(), Operation.MOD, (long)arg2.Read()));
		}
	}

	public class Negate : Instruction
	{
		public override ushort Opcode => 51;

		public override Type[] Arguments => [typeof(Register)];
		public override string Name => "NEG";

		protected override void Handle(Context ctx)
		{
			var reg = ctx.GetNextArg<Register>();
			reg.Write((ushort)-(short)reg.Read());
		}
	}
}
