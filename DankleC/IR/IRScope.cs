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

		private readonly List<int> preservedRegs = [];
		private int usedStack = 0;

		private int varReg = regStart;

		public Variable AllocLocal(string name, TypeSpecifier type)
		{
			if (((END_VAR_REG - varReg + 1) * 2) - type.Size >= 0)
			{
				var regs = new List<int>();

				for (var i = 0; i < Math.Ceiling(type.Size / 2.0); i++)
				{
					regs.Add(varReg + i);
					preservedRegs.Add(varReg + i);
				}

				varReg += regs.Count;

				var variable = new RegisterVariable(name, type, [.. regs], this);
				Locals.Add(variable);
				return variable;
			}
			else
			{
				//var variable = new StackVariable(name, type, new(usedStack,))
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

		public void End()
		{
			ushort regs = 0;
			foreach (var i in preservedRegs)
			{
				regs |= (ushort)(1 << 15 - i);
			}
			Builder.CurrentFunction.Insns.Insert(StartIndex, new PushRegs(regs));
			Builder.CurrentFunction.Insns.Add(new PopRegs(regs));
		}
	}
}
