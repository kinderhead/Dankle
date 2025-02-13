using DankleC.ASTObjects.Expressions;
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
		public ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope);
	}

	public abstract class ResolvedExpression(TypeSpecifier type) : IExpression
	{
		public readonly TypeSpecifier Type = type;

		public ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => this;

		public abstract ResolvedExpression ChangeType(TypeSpecifier type);
		public abstract IValue Execute(IRBuilder builder, IRScope scope);

		public ResolvedExpression Cast(TypeSpecifier type)
		{
			if (Type == type) return this;
			//else if (Type is PointerTypeSpecifier || type is PointerTypeSpecifier) throw new NotImplementedException();
			else if (Type is BuiltinTypeSpecifier actual && type is BuiltinTypeSpecifier expected)
			{
				if (actual.Size == expected.Size) return ChangeType(expected);
				return AsCasted(expected);
			}

			throw new InvalidOperationException($"Cannot cast {Type} to {type}");
		}

		protected virtual ResolvedExpression AsCasted(TypeSpecifier type) => new CastExpression(this, type);

		public SimpleRegisterValue ReturnValue() => new(IRInsn.FitRetRegs(Type.Size), Type);
	}

	public abstract class UnresolvedExpression : IExpression
	{
		public T Resolve<T>(IRBuilder builder, IRFunction func, IRScope scope) where T : ResolvedExpression => (T)Resolve(builder, func, scope);
		public abstract ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope);
	}
}
