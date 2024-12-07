using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public interface IExpression : IASTObject
	{
		public TypeSpecifier? GetTypeSpecifier();
		public ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope);
	}

	public abstract class ResolvedExpression(TypeSpecifier type) : IExpression
	{
		public readonly TypeSpecifier Type = type;

		public TypeSpecifier? GetTypeSpecifier() => Type;
		public ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => this;

		public abstract void WriteToRegisters(int[] regs, IRBuilder builder);

		public abstract ResolvedExpression ChangeType(TypeSpecifier type);
	}

	public class ConstantExpression(TypeSpecifier type, object value) : ResolvedExpression(type)
	{
		public readonly object Value = value;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ConstantExpression(type, Value);

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			if (Type.Size / 2 != regs.Length) throw new InvalidOperationException("Mismatched expression write");

			if (Value is uint ui)
			{
				builder.Add(new LoadImmToReg(0, (ushort)(ui >>> 16)));
				builder.Add(new LoadImmToReg(1, (ushort)(ui & 0xFFFF)));
			}
			else if (Value is int i)
			{
				builder.Add(new LoadImmToReg(0, (ushort)(i >>> 16)));
				builder.Add(new LoadImmToReg(1, (ushort)(i & 0xFFFF)));
			}
			else throw new NotImplementedException();
		}
	}
}
