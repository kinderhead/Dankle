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
		public readonly int Regnum = regnum >= 16 || regnum < 0 ? throw new InvalidOperationException($"Invalid register {regnum}") : regnum;
		public override Type ArgType => typeof(Register);

		public override string Build()
		{
			return $"r{Regnum}";
		}
	}

	public class CGDoubleRegister(int reg1, int reg2) : CGArg<ushort>
	{
		public readonly int Reg1 = reg1 >= 16 || reg1 < 0 ? throw new InvalidOperationException($"Invalid register {reg1}") : reg1;
		public readonly int Reg2 = reg2 >= 16 || reg2 < 0 ? throw new InvalidOperationException($"Invalid register {reg2}") : reg2;
		public override Type ArgType => typeof(DoubleRegister);

		public override string Build()
		{
			return $"(r{Reg1}, r{Reg2})";
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
