using Dankle.Components.Arguments;
using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.CodeGen
{
	public class CGInsn
	{
		public Instruction Insn;
		public ICGArg[] Args;

		public string? Comment = null;

		private CGInsn(Instruction insn, ICGArg[] args)
		{
			Insn = insn;
			Args = args;
		}

		public string Generate()
		{
			var builder = new StringBuilder(Insn.Name.ToLower() + " ");

			foreach (var i in Args)
			{
				builder.Append(i.Build());
				builder.Append(", ");
			}

			if (Args.Length > 0) builder.Length -= 2;
			else builder.Length -= 1;

			if (Comment is not null) builder.Append($" ; {Comment}");

			return builder.ToString();
		}

		public static CGInsn Build<T>(params ICGArg[] args) where T : Instruction, new()
		{
			var insn = new T();

			if (args.Length != insn.Arguments.Length) throw new InvalidOperationException("Mismatched number of arguments for insn " + insn.Name);

			for (int i = 0; i < args.Length; i++)
			{
				var expected = IArgument.Create(insn.Arguments[i]);
				if (expected.GetType() != args[i].ArgType && Array.IndexOf(expected.AssignableFrom, args[i].ArgType) == -1) throw new InvalidOperationException($"Argument {args[i].ArgType.GetType().Name} cannot be assigned to {expected.GetType().Name}");
			}

			return new(insn, args);
		}
	}
}
