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
		public ResolvedExpression Resolve(IRBuilder builder);
	}

	public abstract class ResolvedExpression(TypeSpecifier type) : IExpression
	{
		public readonly TypeSpecifier Type = type;
		public abstract bool IsSimpleExpression { get; }

		public ResolvedExpression Resolve(IRBuilder builder) => this;

		public abstract ResolvedExpression ChangeType(TypeSpecifier type);
		public abstract IValue Execute(IRBuilder builder);
		public abstract void Walk(Action<ResolvedExpression> cb);

		public virtual void Conditional(IRBuilder builder, bool negate = false)
		{
			var cond = new EqualityExpression(this, EqualityOperation.NotEquals, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), (byte)0));
			cond.Resolve(builder).Conditional(builder, negate);
		}

		public virtual ResolvedExpression Standalone() => this;

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
		public T Resolve<T>(IRBuilder builder) where T : ResolvedExpression => (T)Resolve(builder);
		public abstract ResolvedExpression Resolve(IRBuilder builder);
	}
}
