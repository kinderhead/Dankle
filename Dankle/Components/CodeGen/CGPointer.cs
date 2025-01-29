using Dankle.Components.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.CodeGen
{
	public abstract class CGPointer : ICGArg
	{
		public abstract Type ArgType { get; }
		public string Build() => PreBuild().Replace("r12, r13", "SP").Replace("r14, r15", "PC");
		protected abstract string PreBuild();
	}

	public abstract class CGPointer<T> : CGPointer where T : IBinaryInteger<T>
	{
		public override Type ArgType => typeof(Pointer<T>);

		public class Ptr2Reg(int reg1, int reg2) : CGPointer<T>
		{
			protected override string PreBuild() => $"[r{reg1}, r{reg2}]";
		}

		public class Ptr2RegShortOffset(int reg1, int reg2, short offset) : CGPointer<T>
		{
			protected override string PreBuild() => $"[r{reg1}, r{reg2} + 0x{offset:X4}]";
		}

		public static Ptr2Reg Make(int reg1, int reg2) => new(reg1, reg2);
		public static Ptr2RegShortOffset Make(int reg1, int reg2, short offset) => new(reg1, reg2, offset);
	}
}
