using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public class IRFunction(string name, TypeSpecifier returnType, ParameterList parameters)
	{
		public readonly string Name = name;
		public readonly TypeSpecifier ReturnType = returnType;
		public readonly ParameterList Parameters = parameters;
		public readonly List<IRInsn> Insns = [];

		public string SymbolName { get => "_" + Name; }
	}
}
