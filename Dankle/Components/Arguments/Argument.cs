using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public interface IArgument
	{
		public string Dissassemble();

		public IArgument Create(Context ctx, int argnum);

		public static IArgument Create(Type type, Context ctx, int argnum)
		{
			return (IArgument?)type.GetMethod("Create")?.Invoke(Activator.CreateInstance(type), [ctx, argnum]) ?? throw new Exception("Could not create argument");
		}
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

		public abstract string Dissassemble();
	}
}
