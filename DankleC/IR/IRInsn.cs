using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public abstract class IRInsn
	{
		public abstract void Compile(CodeGen gen);
	}

	public class ReturnInsn : IRInsn
	{
		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<Return>());
		}
	}

	public class LoadImmToReg(int reg, ushort val) : IRInsn
	{
		public readonly int Register = reg;
		public readonly ushort Val = val;

		public override void Compile(CodeGen gen)
		{
			gen.Add(CodeGenInsn.Build<Load>(new CodeGenRegister(Register), new CodeGenImmediate<ushort>(Val)));
		}
	}
}
