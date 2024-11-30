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

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "ADD";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.ADD, arg2.Read()));
		}
	}

	public class Adc : Instruction
	{
		public override ushort Opcode => 35;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "ADC";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			var carry = ctx.Core.Carry;

			var val = ctx.Core.ALU.Calculate(arg1.Read(), Operation.ADD, arg2.Read());
			if (carry) val = ctx.Core.ALU.Calculate(val, Operation.ADD, (ushort)1);
			dest.Write(val);
		}
	}

	public class Subtract : Instruction
	{
		public override ushort Opcode => 8;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "SUB";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.SUB, arg2.Read()));
		}
	}

	public class Sbb : Instruction
	{
		public override ushort Opcode => 39;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "SBB";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			var carry = ctx.Core.Carry;

			var val = ctx.Core.ALU.Calculate(arg1.Read(), Operation.SUB, arg2.Read());
			if (carry) val = ctx.Core.ALU.Calculate(val, Operation.SUB, (ushort)1);
			dest.Write(val);
		}
	}

	public class SignedMul : Instruction
	{
		public override ushort Opcode => 9;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "SMUL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write((ushort)ctx.Core.ALU.Calculate((short)arg1.Read(), Operation.MUL, (short)arg2.Read()));
		}
	}

	public class SignedDiv : Instruction
	{
		public override ushort Opcode => 10;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "SDIV";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write((ushort)ctx.Core.ALU.Calculate((short)arg1.Read(), Operation.DIV, (short)arg2.Read()));
		}
	}

	public class UnsignedMul : Instruction
	{
		public override ushort Opcode => 11;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "UMUL";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.MUL, arg2.Read()));
		}
	}

	public class UnsignedDiv : Instruction
	{
		public override ushort Opcode => 12;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "UDIV";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

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

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "LSH";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Shift(arg1.Read(), ShiftOperation.LSH, arg2.Read()));
		}
	}

	public class RightShift : Instruction
	{
		public override ushort Opcode => 14;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "RSH";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Shift(arg1.Read(), ShiftOperation.RSH, arg2.Read()));
		}
	}

	public class Or : Instruction
	{
		public override ushort Opcode => 25;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "OR";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Bitwise(arg1.Read(), BitwiseOperation.OR, arg2.Read()));
		}
	}

	public class And : Instruction
	{
		public override ushort Opcode => 26;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "AND";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Bitwise(arg1.Read(), BitwiseOperation.AND, arg2.Read()));
		}
	}

	public class Xor : Instruction
	{
		public override ushort Opcode => 27;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "XOR";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Bitwise(arg1.Read(), BitwiseOperation.XOR, arg2.Read()));
		}
	}

	public class Modulo : Instruction
	{
		public override ushort Opcode => 28;

		public override Type[] Arguments => [typeof(Register), typeof(Register), typeof(Register)];
		public override string Name => "MOD";

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.MOD, arg2.Read()));
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
}
