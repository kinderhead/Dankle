using Dankle.Components.CodeGen;
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

		private string currentFunc = "";

		public string Compile()
		{
			foreach (var func in IR.Functions)
			{
				CompiledSymbols[func.SymbolName] = "";
				currentFunc = func.SymbolName;
				foreach (var insn in func.Insns)
				{
					insn.Compile(this);
				}
			}

			var builder = new StringBuilder();

			foreach (var sym in CompiledSymbols)
			{
				builder.Append($"{sym.Key}:");
				builder.AppendLine(sym.Value);
			}

			return builder.ToString();
		}

		public void Add(CGInsn insn)
		{
			CompiledSymbols[currentFunc] += $"\n    {insn.Generate()}";
		}

		public void AddLabel(string name)
		{
			CompiledSymbols[currentFunc] += $"\n{name}:";
		}
	}
}
