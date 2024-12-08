using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public class ConstantExpression(TypeSpecifier type, object value) : ResolvedExpression(type)
	{
		public readonly object Value = value;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ConstantExpression(type, Value);

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			if (Type.Size / 2 != regs.Length) throw new InvalidOperationException("Mismatched expression write");

			if (Value is uint ui)
			{
				builder.Add(new LoadImmToReg(regs[0], (ushort)(ui >>> 16)));
				builder.Add(new LoadImmToReg(regs[1], (ushort)(ui & 0xFFFF)));
			}
			else if (Value is int i)
			{
				builder.Add(new LoadImmToReg(regs[0], (ushort)(i >>> 16)));
				builder.Add(new LoadImmToReg(regs[1], (ushort)(i & 0xFFFF)));
			}
			else throw new NotImplementedException();
		}
	}
}
