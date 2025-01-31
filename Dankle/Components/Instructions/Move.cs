using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Move : Instruction
	{
		public override ushort Opcode => 6;

		public override Type[] Arguments => [typeof(Register), typeof(Register)];
		public override string Name => "MOV";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Register>();
			var src = ctx.GetNextArg<Register>();

			dest.Write(src.Read());
		}
	}

	public class SignExtend : Instruction
	{
		public override ushort Opcode => 56;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "SXT";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any16>();
			var src = ctx.GetNextArg<Any16>();

			if ((short)src.Read() < 0) dest.Write(0xFFFF);
			else dest.Write(0);
		}
	}

	public class SignExtend8 : Instruction
	{
		public override ushort Opcode => 57;

		public override Type[] Arguments => [typeof(Any16), typeof(Any16)];
		public override string Name => "SXT8";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Any16>();
			var src = ctx.GetNextArg<Any16>();

			var s = src.Read();
			if ((sbyte)s < 0) dest.Write((ushort)(s & 0xFF00));
			else dest.Write(s);
		}
	}
}
