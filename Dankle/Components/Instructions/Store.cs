using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Store : Instruction
	{
		public override ushort Opcode => 3;

		public override Type[] Arguments => [typeof(Pointer<ushort>), typeof(Register)];
		public override string Name => "ST";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Pointer<ushort>>();
			var src = ctx.GetNextArg<Register>();

			dest.Write(src.Read());
		}
	}

	public class Store8 : Instruction
	{
		public override ushort Opcode => 5;

		public override Type[] Arguments => [typeof(Pointer<byte>), typeof(Register)];
		public override string Name => "STB";

		protected override void Handle(Context ctx)
		{
			var dest = ctx.GetNextArg<Pointer<byte>>();
			var src = ctx.GetNextArg<Register>();

			dest.Write((byte)src.Read());
		}
	}
}
