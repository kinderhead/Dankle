﻿using Dankle.Components.CodeGen;
using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC
{
	public class CodeGen(IRBuilder ir)
	{
		public readonly IRBuilder IR = ir;
		public readonly Dictionary<string, List<InsnDef>> CompiledSymbols = [];
		public readonly List<string> ExportedFunctions = [];

		public string CurrentFunc { get; private set; } = "";

		private int logicLabelCounter = 0;

		public string Compile(Optimizer.Settings? settings = null)
		{
			foreach (var func in IR.Functions)
			{
				CompiledSymbols[func.SymbolName] = [];
				if (!func.Type.IsStatic) ExportedFunctions.Add(func.SymbolName);
				CurrentFunc = func.SymbolName;
				foreach (var insn in func.Insns)
				{
					insn.Compile(this);
				}

				foreach (var insn in func.Insns)
				{
					insn.PostCompile(this);
					CompiledSymbols[CurrentFunc].AddRange(insn.Insns);
				}
			}

			var optimizer = new Optimizer(settings ?? new(true, false));
			var builder = new StringBuilder();

			foreach (var sym in ExportedFunctions)
			{
				builder.AppendLine($"export {sym}");
			}

			foreach (var sym in IR.ExternsUsed)
			{
				builder.AppendLine($"import {sym}");
			}

			foreach (var sym in CompiledSymbols)
			{
				builder.Append($"\n{sym.Key}:");

				optimizer.Optimize(sym.Value);
				for (var i = 0; i < sym.Value.Count; i++)
				{
					if (sym.Value[i].Insn is not null) builder.Append($"\n    {sym.Value[i].Insn?.Generate()}");
					else builder.Append($"\n{sym.Value[i].Label}:");
				}

				builder.Append('\n');
			}

			foreach (var i in IR.StaticVariables)
			{
				builder.Append($"\n{i.Value.Item1.Resolve(this)}:\n    {string.Join(' ', i.Value.Item2.Select(e => $"0x{e:X2}"))}");
			}

			foreach (var i in IR.Literals)
			{
				builder.Append($"\n{i.Label?.Resolve(this)}:\n{i.Build(this)}");
			}

			return builder.ToString();
		}

		public string GetLogicLabel()
		{
			return $"L${logicLabelCounter++}";
		}
	}
}
