using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public class IRScope(ScopeNode scope, IRBuilder builder)
	{
		public const int MAX_VAR_REG = 7;

		public readonly ScopeNode Scope = scope;
		public readonly IRBuilder Builder = builder;
		public readonly List<Variable> Locals = [];

		private int varReg = 4;

		public Variable AllocLocal(string name, TypeSpecifier type)
		{
			if (((MAX_VAR_REG - varReg + 1) * 2) - type.Size > 0)
			{
				var regs = new List<int>();

				for (var i = 0; i < Math.Ceiling(type.Size / 2.0); i++)
				{
					regs.Add(varReg + i);
				}

				varReg += regs.Count;

				var variable = new RegisterVariable(name, type, [.. regs], this);
				Locals.Add(variable);
				return variable;
			}

			throw new NotImplementedException();
		}

		public Variable GetVariable(string name)
		{
			foreach (var i in Locals)
			{
				if (i.Name == name) return i;
			}

			throw new Exception($"Could not find variable with name {name}");
		}
	}
}
