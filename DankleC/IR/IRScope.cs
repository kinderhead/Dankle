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
		public short EffectiveStackUsed { get => (short)(StackUsed + MaxTempStackUsed); }
		public short StackUsed { get; private set; } = 0;
		public short MaxTempStackUsed { get; private set; } = 0;

		private int varReg = regStart;
		private short tempStackUsed = 0;
		private readonly List<int> tempRegsUsed = [];

		private readonly List<string> RequiredStackAllocVariables = [];

		public Variable AllocLocal(string name, TypeSpecifier type)
		{
			if (!RequiredStackAllocVariables.Contains(name) && ((END_VAR_REG - varReg + 1) * 2) - type.Size >= 0)
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
			else return AllocStackLocal(name, type);
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

		public TempRegHolder AllocTempRegs(int bytes)
		{
			var count = IRBuilder.NumRegForBytes(bytes);

			var freeRegs = new List<int>();
			var usedRegs = new List<int>();
			for (int i = 0; i < count; i++)
			{
				var found = false;
				for (var reg = 8; reg < 12; reg++)
				{
					if (!tempRegsUsed.Contains(reg) && !freeRegs.Contains(reg))
					{
						freeRegs.Add(reg);
						found = true;
						break;
					}
				}

				if (!found)
				{
					for (var reg = 8; reg < 12; reg++)
					{
						if (tempRegsUsed.Contains(reg) && !usedRegs.Contains(reg))
						{
							usedRegs.Add(reg);
							break;
						}
					}
				}
			}

			if (freeRegs.Count + usedRegs.Count != count) throw new InvalidOperationException();
			return new(Builder, this, [.. freeRegs], [.. usedRegs]);
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

		public class TempRegHolder : IDisposable
		{
			public readonly IRBuilder Builder;
			public readonly IRScope Scope;
			public readonly int[] FreeRegs;
			public readonly int[] UsedRegs;
			public readonly TempStackVariable? Storage;

			public TempRegHolder(IRBuilder builder, IRScope scope, int[] freeRegs, int[] usedRegs)
			{
				Builder = builder;
				Scope = scope;
				FreeRegs = freeRegs;
				UsedRegs = usedRegs;

				scope.tempRegsUsed.AddRange(freeRegs);

				if (usedRegs.Length > 0)
				{
					Storage = scope.AllocTemp(TypeSpecifier.GetGenericForSize(usedRegs.Length * 2));
					builder.MoveRegsToPtr(usedRegs, Storage.Pointer);
				}
			}

			public int[] Registers { get => [.. FreeRegs, .. UsedRegs]; }

			public void Dispose()
			{
				Scope.tempRegsUsed.RemoveAll(i => FreeRegs.Contains(i));

				if (Storage is not null)
				{
					Builder.MovePtrToRegs(Storage.Pointer, UsedRegs);
					Storage.Dispose();
				}
				GC.SuppressFinalize(this);
			}
		}
	}
}
