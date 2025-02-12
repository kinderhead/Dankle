using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public class IRScope(ScopeNode scope, IRBuilder builder, int startIndex)
	{
		public const int END_VAR_REG = 7;
		public const int START_VAR_REG = 4;

		public readonly ScopeNode Scope = scope;
		public readonly IRBuilder Builder = builder;
		public readonly int StartIndex = startIndex;

		public readonly List<Variable> Locals = [];

		public short EffectiveStackUsed { get => (short)(StackUsed + MaxTempStackUsed); }
		public short StackUsed { get; private set; } = 0;
		public short MaxTempStackUsed { get; private set; } = 0;

		private short tempStackUsed = 0;

		private readonly List<string> RequiredStackAllocVariables = [];

		public Variable AllocLocal(string name, TypeSpecifier type)
		{
			return AllocStackLocal(name, type);
		}

		public StackVariable AllocStackLocal(string name, TypeSpecifier type)
		{
			var variable = new StackVariable(name, type, new StackPointer(StackUsed, type.Size), this);
			StackUsed = checked((short)(StackUsed + type.Size));
			Locals.Add(variable);
			return variable;
		}

		public TempStackVariable AllocTemp(TypeSpecifier type)
		{
			var temp = new TempStackVariable($"_{NameGen.Next()}", type, new TempStackPointer(tempStackUsed, type.Size), this);
			tempStackUsed = checked((short)(tempStackUsed + type.Size));
			MaxTempStackUsed = Math.Max(tempStackUsed, MaxTempStackUsed);
			return temp;
		}

		public void FreeTemp(TempStackVariable temp)
		{
			tempStackUsed = checked((short)(tempStackUsed - temp.Type.Size));
		}

		public void RequireStackAlloc(string name) => RequiredStackAllocVariables.Add(name);

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

		private static readonly Random NameGen = new();
	}
}
