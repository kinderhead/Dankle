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

				foreach (var insn in func.Insns)
				{
					insn.PostCompile(this);

					foreach (var cg in insn.Insns)
					{
						CompiledSymbols[currentFunc] += $"\n    {cg.Generate()}";
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
