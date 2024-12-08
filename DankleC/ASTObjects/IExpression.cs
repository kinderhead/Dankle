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

	public abstract class UnresolvedExpression : IExpression
	{
		public TypeSpecifier? GetTypeSpecifier() => null;
		public abstract ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope);
	}
}
