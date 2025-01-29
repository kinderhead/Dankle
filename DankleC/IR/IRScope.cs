using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public class IRScope(ScopeNode scope, IRBuilder builder, int startIndex, int regStart = IRScope.START_VAR_REG)
	{
		public const int END_VAR_REG = 7;
		public const int START_VAR_REG = 4;

		public readonly ScopeNode Scope = scope;
		public readonly IRBuilder Builder = builder;
		public readonly int StartIndex = startIndex;

		public readonly List<Variable> Locals = [];

		public List<int> PreservedRegs { get; private set; } = [];
		public short StackUsed { get; private set; } = 0;

		private int varReg = regStart;

		public Variable AllocLocal(string name, TypeSpecifier type)
		{
			if (((END_VAR_REG - varReg + 1) * 2) - type.Size >= 0)
			{
				var regs = new List<int>();

				for (var i = 0; i < Math.Ceiling(type.Size / 2.0); i++)
				{
					regs.Add(varReg + i);
					PreservedRegs.Add(varReg + i);
				}

				varReg += regs.Count;

				var variable = new RegisterVariable(name, type, [.. regs], this);
				Locals.Add(variable);
				return variable;
			}
			else
			{
				var variable = new StackVariable(name, type, new(StackUsed, type.Size), this);
				StackUsed += (short)type.Size;
				Locals.Add(variable);
				return variable;
			}
		}

		public Variable GetVariable(string name)
		{
			foreach (var i in Locals)
			{
				if (i.Name == name) return i;
			}

			throw new Exception($"Could not find variable with name {name}");
		}

		public void Start()
        {
			Builder.Add(new InitFrame());
        }

		public void End()
		{
			Builder.Add(new EndFrame());
		}
    }
}
