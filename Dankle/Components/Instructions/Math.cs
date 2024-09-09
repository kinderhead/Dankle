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

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.ADD, arg2.Read()));
		}
	}

	public class Subtract : Instruction
	{
		public override ushort Opcode => 8;

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.SUB, arg2.Read()));
		}
	}

	public class SignedMul : Instruction
	{
		public override ushort Opcode => 9;

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

		protected override void Handle(Context ctx)
		{
			var arg1 = ctx.GetNextArg<Register>();
			var arg2 = ctx.GetNextArg<Register>();
			var dest = ctx.GetNextArg<Register>();

			dest.Write(ctx.Core.ALU.Calculate(arg1.Read(), Operation.DIV, arg2.Read()));
		}
	}
}
