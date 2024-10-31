using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Immediate<T> : Argument<T> where T : IBinaryInteger<T>
	{
		public Immediate()
		{
		}

		public Immediate(Context ctx, int type) : base(ctx, type)
		{
		}

		public override IArgument Create(Context ctx, int argnum) => new Immediate<T>(ctx, argnum);

		public override T Read() => Ctx.Core.GetNext<T>();

		public override void Write(T value) => throw new InvalidOperationException("Cannot write to an immediate value");
	}
}
