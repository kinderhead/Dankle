using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public class IRFunction(string name, FunctionTypeSpecifier type)
	{
		public readonly string Name = name;
		public readonly FunctionTypeSpecifier Type = type;
		public readonly List<IRInsn> Insns = [];

		public string SymbolName { get => "_" + Name; }
	}
}
