﻿using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public class IRScope(ScopeNode scope, IRBuilder builder, int startIndex)
	{
		public readonly ScopeNode Scope = scope;
		public readonly IRBuilder Builder = builder;
		public readonly int StartIndex = startIndex;

		private readonly List<List<Variable>> Locals = [[]];

		public int EffectiveStackUsed { get => StackUsed + MaxTempStackUsed + MaxFuncAllocStackUsed; }
		public int StackUsed { get; private set; } = 0;
		public int MaxTempStackUsed { get; private set; } = 0;
		public int MaxFuncAllocStackUsed { get; private set; } = 0;

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
			Locals.Last().Add(variable);
			return variable;
		}

		public void ReserveFunctionCallSpace(FunctionTypeSpecifier func)
		{
			var stack = 0;
			foreach (var i in func.Parameters)
			{
				stack += i.Size;
			}
			MaxFuncAllocStackUsed = Math.Max(stack, MaxFuncAllocStackUsed);
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

		public void SetupArguments(IRFunction func)
		{
			int offset = 0;
			foreach (var i in func.Parameters.Parameters)
			{
				Locals[0].Add(new StackVariable(i.Item2, i.Item1, new PostArgumentPointer(offset, i.Item1.Size), this));
				offset += i.Item1.Size;
			}
		}

		public Variable GetVariable(string name)
		{
			foreach (var i in Locals.AsEnumerable().Reverse())
			{
				foreach (var e in i)
				{
					if (e.Name == name) return e;
				}
			}

			foreach (var i in Builder.Functions)
			{
				if (i.Name == name) return new FunctionVariable(name, new(i.ReturnType, [.. i.Parameters.Parameters.Select(i => i.Item1)]), this);
			}

			throw new Exception($"Could not find variable with name {name}");
		}

		public void SubScope(Action func)
		{
			Locals.Add([]);
			var lastStackUsed = StackUsed;
			func();
			Locals.RemoveAt(Locals.Count - 1);
			StackUsed = lastStackUsed;
		}

		public void Start()
		{
			Builder.Add(new InitFrame());
		}

		//public void End()
		//{
			
		//}

		private static readonly Random NameGen = new();
	}
}
