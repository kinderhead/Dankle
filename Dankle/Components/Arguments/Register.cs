using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Register(Context ctx, int argnum) : Argument<ushort>(ctx, argnum)
	{
		public override ushort Read() => Ctx.Core.Registers[Ctx.Data[ArgNum]];
		public override void Write(ushort value) => Ctx.Core.Registers[Ctx.Data[ArgNum]] = value;
	}
}
