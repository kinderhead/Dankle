using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Instructions
{
	public class Low : Instruction
	{
		public override ushort Opcode => 36;

		public override Type[] Arguments => [typeof(Register)];

		public override string Name => "low";

		protected override void Handle(Context ctx)
		{
			ctx.Core.Registers.SetState(ctx.Data[0], CPUCore.RegisterHandler.RegisterState.Low);
		}
	}

	public class High : Instruction
	{
		public override ushort Opcode => 37;

		public override Type[] Arguments => [typeof(Register)];

		public override string Name => "high";

		protected override void Handle(Context ctx)
		{
			ctx.Core.Registers.SetState(ctx.Data[0], CPUCore.RegisterHandler.RegisterState.High);
		}
	}

	public class Reset : Instruction
	{
		public override ushort Opcode => 38;

		public override Type[] Arguments => [typeof(Register)];

		public override string Name => "rst";

		protected override void Handle(Context ctx)
		{
			ctx.Core.Registers.SetState(ctx.Data[0], CPUCore.RegisterHandler.RegisterState.None);
		}
	}
}
