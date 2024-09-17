using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public interface IArgument
	{

	}

	public abstract class Argument<T>(Context ctx, int argnum) : IArgument where T : IBinaryInteger<T>
	{
		public readonly Context Ctx = ctx;
		public readonly int ArgNum = argnum;

		public Type ArgType => typeof(T);

		public abstract T Read();
		public abstract void Write(T value);
	}
}
