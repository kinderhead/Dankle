using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Load : Instruction
	{
		public override ushort Opcode => 2;

		public override Type[] Arguments => [typeof(Register), typeof(Any16Num)];
		public override string Name => "LD";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Register>();
			var src = ctx.GetNextArg<Any16Num>();

			dest.Write(src.Read());
		}
	}

	public class Load8 : Instruction
	{
		public override ushort Opcode => 4;

		public override Type[] Arguments => [typeof(Register), typeof(Any8Num)];
		public override string Name => "LDB";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Register>();
			var src = ctx.GetNextArg<Any8Num>();

			dest.Write(src.Read());
		}
	}

	public class Load32 : Instruction
	{
		public override ushort Opcode => 88;

		public override Type[] Arguments => [typeof(Any32), typeof(Any32)];
		public override string Name => "LDL";

		protected override void Handle(Context ctx)
		{
			var src = ctx.GetNextArg<Any32>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write(src.Read());
		}
	}

	public class Load64 : Instruction
	{
		public override ushort Opcode => 90;

		public override Type[] Arguments => [typeof(Any64), typeof(Any64)];
		public override string Name => "LDLL";

		protected override void Handle(Context ctx)
		{
			var src = ctx.GetNextArg<Any64>();
			var dest = ctx.GetNextArg<Any64>();

			dest.Write(src.Read());
		}
	}

	public class LoadEffectiveAddress : Instruction
	{
		public override ushort Opcode => 60;

		public override Type[] Arguments => [typeof(Pointer<ushort>), typeof(Any32)];
		public override string Name => "LEA";

		protected override void Handle(Context ctx)
		{
			var ptr = ctx.GetNextArg<Pointer<ushort>>();
			var dest = ctx.GetNextArg<Any32>();

			dest.Write(ptr.GetAddress());
		}
	}

	public class GetCompare : Instruction
	{
		public override ushort Opcode => 61;

		public override Type[] Arguments => [typeof(Register)];
		public override string Name => "GETC";

		protected override void Handle(Context ctx)
		{
			var reg = ctx.GetNextArg<Register>();

			reg.Write((ushort)(ctx.Core.Compare ? 1 : 0));
		}
	}

	public class GetNotCompare : Instruction
	{
		public override ushort Opcode => 62;

		public override Type[] Arguments => [typeof(Register)];
		public override string Name => "GETNC";

		protected override void Handle(Context ctx)
		{
			var reg = ctx.GetNextArg<Register>();

			reg.Write((ushort)(ctx.Core.Compare ? 0 : 1));
		}
	}
}
