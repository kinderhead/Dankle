using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Register : Argument<ushort>
	{
		private readonly bool UseArgNumAsRegister = false;

		public Register(Context ctx, int argnum) : base(ctx, argnum)
		{
		}

		public Register(Context ctx, int argnum, bool useArgNumAsRegister) : base(ctx, argnum)
		{
			UseArgNumAsRegister = useArgNumAsRegister;
		}

		public override ushort Read() => Ctx.Core.Registers[UseArgNumAsRegister ? ArgNum : Ctx.Data[ArgNum]];
		public override void Write(ushort value) => Ctx.Core.Registers[UseArgNumAsRegister ? ArgNum : Ctx.Data[ArgNum]] = value;
	}

	public class DoubleRegister(Context ctx, int argnum) : Argument<uint>(ctx, argnum)
	{
		public override uint Read()
		{
			var data = Ctx.Core.GetNext<byte>();
			return Utils.Merge(Ctx.Core.Registers[data >> 4], Ctx.Core.Registers[data & 0xF]);
		}

		public override void Write(uint value)
		{
			var data = Ctx.Core.GetNext<byte>();
			Ctx.Core.Registers[data >> 4] = (ushort)(value >> 16);
			Ctx.Core.Registers[data & 0xF] = (ushort)value;
		}
	}
}
