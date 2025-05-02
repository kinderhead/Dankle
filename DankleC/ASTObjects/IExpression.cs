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
		public ResolvedExpression Cast(TypeSpecifier type);
	}

	public abstract class ResolvedExpression(TypeSpecifier type) : IExpression
	{
		public readonly TypeSpecifier Type = type;
		public abstract bool IsSimpleExpression { get; }
		public virtual bool CanAnyCast => false;

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
			else if (CanAnyCast) return ChangeType(type);
			else if (type is BuiltinTypeSpecifier v && v.Type == BuiltinType.Void) return Standalone();
			//else if (Type is PointerTypeSpecifier || type is PointerTypeSpecifier) throw new NotImplementedException();
			else if (Type is BuiltinTypeSpecifier actual && type is BuiltinTypeSpecifier expected)
			{
				if (actual.Size == expected.Size) return ChangeType(expected);
				return AsCasted(expected);
			}
			else if (Type is ArrayTypeSpecifier arr && type is PointerTypeSpecifier ptr && (arr.Inner == ptr.Inner || ptr.Inner.Size == 0))
			{
				return new ResolvedRefExpression((LValue)this, type);
			}
			else if (Type is FunctionTypeSpecifier func && type is PointerTypeSpecifier ptr2 && func == ptr2.Inner)
			{
				return new ResolvedRefExpression((LValue)this, type);
			}

			throw new InvalidOperationException($"Cannot cast {Type} to {type}");
		}

		protected virtual ResolvedExpression AsCasted(TypeSpecifier type) => new CastExpression(this, type);

		public IValue ReturnValue(IRBuilder builder)
		{
			if (Type.IsNumber()) return new SimpleRegisterValue(IRInsn.FitRetRegs(Type.Size), Type);
			else return new SimplePointerValue(IRInsn.GetReturnPointer(Type.Size), Type, builder.CurrentScope);
		}
	}

	public abstract class UnresolvedExpression : IExpression
	{
		public ResolvedExpression Cast(TypeSpecifier type) => throw new InvalidOperationException();
        public T Resolve<T>(IRBuilder builder) where T : ResolvedExpression => (T)Resolve(builder);
		public abstract ResolvedExpression Resolve(IRBuilder builder);
	}
}
