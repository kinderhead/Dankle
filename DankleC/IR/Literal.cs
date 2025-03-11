using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
    public abstract class Literal
    {
		public IRLabel? Label;

        public abstract string Build(CodeGen gen);
	}

	public class StringLiteral(string text) : Literal
	{
		public readonly string Text = text;

		public override string Build(CodeGen gen)
		{
			return $"    \"{Text}\"";
		}
	}
}
