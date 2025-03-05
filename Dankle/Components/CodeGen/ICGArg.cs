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

		public override bool Equals(object? obj) => obj is CGRegister reg && reg.Regnum == Regnum;
        public override int GetHashCode() => base.GetHashCode();
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

		public override bool Equals(object? obj) => obj is CGDoubleRegister reg && reg.Reg1 == Reg1 && reg.Reg2 == Reg2;
        public override int GetHashCode() => base.GetHashCode();
	}

	public class CGQuadRegister(int reg1, int reg2, int reg3, int reg4) : CGArg<ushort>
	{
		public readonly int Reg1 = reg1 >= 16 || reg1 < 0 ? throw new InvalidOperationException($"Invalid register {reg1}") : reg1;
		public readonly int Reg2 = reg2 >= 16 || reg2 < 0 ? throw new InvalidOperationException($"Invalid register {reg2}") : reg2;
		public readonly int Reg3 = reg3 >= 16 || reg3 < 0 ? throw new InvalidOperationException($"Invalid register {reg3}") : reg3;
		public readonly int Reg4 = reg4 >= 16 || reg4 < 0 ? throw new InvalidOperationException($"Invalid register {reg4}") : reg4;
		public override Type ArgType => typeof(QuadRegister);

		public override string Build()
		{
			return $"(r{Reg1}, r{Reg2}, r{Reg3}, r{Reg4})";
		}

		public override bool Equals(object? obj) => obj is CGQuadRegister reg && reg.Reg1 == Reg1 && reg.Reg2 == Reg2 && reg.Reg3 == Reg3 && reg.Reg4 == Reg4;
		public override int GetHashCode() => base.GetHashCode();
	}

	public class CGImmediate<T>(T val) : CGArg<T> where T : IBinaryInteger<T>
	{
		public readonly T Value = val;
		public override Type ArgType => typeof(Immediate<T>);

		public override string Build()
		{
			return $"{Value}";
		}

		public override bool Equals(object? obj) => obj is CGImmediate<T> imm && imm.Value == Value;
        public override int GetHashCode() => base.GetHashCode();
	}

	public class CGLabel<T>(string name) : CGArg<T> where T : IBinaryInteger<T>
	{
		public readonly string Name = name;
		public override Type ArgType => typeof(Immediate<T>);

		public override string Build()
		{
			return $"{Name}";
		}

		public override bool Equals(object? obj) => obj is CGLabel<T> label && label.Name == Name;
        public override int GetHashCode() => base.GetHashCode();
	}
}
