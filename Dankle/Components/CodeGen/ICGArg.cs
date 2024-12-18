using Dankle.Components.Arguments;
using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.CodeGen
{
	public interface ICGArg
	{
		public Type ArgType { get; }

		public string Build();
	}

	public abstract class CGArg<T> : ICGArg where T : IBinaryInteger<T>
	{
		public abstract Type ArgType { get; }

		public abstract string Build();
	}

	public class CGRegister(int regnum) : CGArg<ushort>
	{
		public readonly int Regnum = regnum >= 16 ? throw new InvalidOperationException($"Invalid register {regnum}") : regnum;
		public override Type ArgType => typeof(Register);

		public override string Build()
		{
			return $"r{Regnum}";
		}
	}

	public class CGImmediate<T>(T val) : CGArg<T> where T : IBinaryInteger<T>
	{
		public readonly T Value = val;
		public override Type ArgType => typeof(Immediate<T>);

		public override string Build()
		{
			return $"{Value}";
		}
	}
}
