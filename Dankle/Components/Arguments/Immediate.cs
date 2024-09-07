using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Immediate<T>(Context ctx, int type) : Argument<T>(ctx, type) where T : IBinaryInteger<T>
	{
		public override T Read() => Ctx.Core.GetNext<T>();

		public override void Write(T value) => throw new InvalidOperationException("Cannot write to an immediate value");
	}
}
