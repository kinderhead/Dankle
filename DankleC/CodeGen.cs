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
		public readonly Dictionary<string, string> CompiledSymbols = [];

		public string CurrentFunc { get; private set; } = "";

		public string Compile()
		{
			foreach (var func in IR.Functions)
			{
				CompiledSymbols[func.SymbolName] = "";
				CurrentFunc = func.SymbolName;
				foreach (var insn in func.Insns)
				{
					insn.Compile(this);
				}

				foreach (var insn in func.Insns)
				{
					insn.PostCompile(this);

					for (var i = 0; i < insn.Insns.Count; i++)
					{
						CompiledSymbols[CurrentFunc] += $"\n    {insn.Insns[i].Generate()}";
					}
				}
			}

			var builder = new StringBuilder();

			foreach (var sym in CompiledSymbols)
			{
				builder.AppendLine($"export {sym.Key}");
			}

			foreach (var sym in CompiledSymbols)
			{
				builder.Append($"{sym.Key}:");
				builder.AppendLine(sym.Value);
			}

			return builder.ToString();
		}
	}
}
