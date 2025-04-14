using Dankle.Components.CodeGen;
using DankleC.IR;
using ShellProgressBar;
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
		public readonly List<string> Exports = [];

		public string CurrentFunc { get; private set; } = "";

		private int logicLabelCounter = 0;

		public string Compile(Optimizer.Settings? settings = null, ProgressBar? pb = null)
		{
			var child = pb?.Spawn(IR.Functions.Count, "Compiling functions...");
			foreach (var func in IR.Functions)
			{
				if (child is not null) child.Message = $"Compiling {func.Name}...";

				CompiledSymbols[func.SymbolName] = [];
				if (!func.Type.IsStatic) Exports.Add(func.SymbolName);
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

				child?.Tick();
			}
			child?.Dispose();

			foreach (var i in IR.GlobalVariables)
			{
				if (!i.Value.Innermost.IsStatic) Exports.Add($"_{i.Key}");
			}

			var optimizer = new Optimizer(settings ?? new(true, false));
			var builder = new StringBuilder();

			foreach (var sym in Exports)
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
				builder.Append($"\n{i.Value.Item1.Resolve(this)}:\n    {i.Value.Item2.Compile()}");
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
