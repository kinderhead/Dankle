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
		public abstract string Build();
	}

	public abstract class CGPointer<T> : CGPointer where T : IBinaryInteger<T>
	{
		public override Type ArgType => typeof(Pointer<T>);

		public class Ptr2RegIntOffset(int reg1, int reg2, short offset) : CGPointer<T>
		{
			public override string Build() => $"[{reg1}, {reg2} + {offset}]";
		}

		public static Ptr2RegIntOffset Make(int reg1, int reg2, short offset) => new(reg1, reg2, offset);
	}
}
