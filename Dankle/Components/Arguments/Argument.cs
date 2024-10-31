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
		public IArgument Create(Context ctx, int argnum);
	}

	public abstract class Argument<T> : IArgument where T : IBinaryInteger<T>
	{
		public readonly Context Ctx;
		public readonly int ArgNum;

		public Argument(Context ctx, int argnum)
		{
			Ctx = ctx;
			ArgNum = argnum;
		}

		public Argument()
		{
		}

		public Type ArgType => typeof(T);

		public abstract IArgument Create(Context ctx, int argnum);

		public abstract T Read();
		public abstract void Write(T value);
	}
}
