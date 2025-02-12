﻿using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
#pragma warning disable CS8618
	public class IRBuilder(ProgramNode ast, bool debug = false)
	{
		public readonly ProgramNode AST = ast;
		public readonly bool Debug = debug;

		public readonly List<IRFunction> Functions = [];

		public IRFunction CurrentFunction { get; private set; }
		public IRScope CurrentScope { get; private set; }

		private int logicLabelCounter = 0;

		public void Build()
		{
			foreach (var i in AST.Functions)
			{
				HandleFunction(i);
			}
		}

		private void HandleFunction(FunctionNode node)
		{
			var func = new IRFunction(node.Name, node.ReturnType);

			CurrentFunction = func;

			HandleScope(func, new(node.Scope, this, 0));
			// func.Insns.Add(new ReturnInsn());

			Functions.Add(func);
		}

		public void HandleScope(IRFunction func, IRScope scope)
		{
			CurrentScope = scope;
			scope.Start();
			foreach (var i in scope.Scope.Statements)
			{
				i.Scope = scope;
				if (Debug) Add(new IRLabel($"stmt_{i.ID}"));
				i.BuildIR(this, func);
			}
			// scope.End();
		}

		public void Add(IRInsn insn)
		{
			insn.Scope = CurrentScope;
			CurrentFunction.Insns.Add(insn);
		}

		public IRLabel GetLogicLabel()
		{
			var name = $"L${logicLabelCounter++}";
			return new IRLabel(name);
		}

		public static int NumRegForBytes(int bytes) => (int)Math.Ceiling(bytes / 2.0);
	}
#pragma warning restore CS8618
}
