﻿using DankleC.ASTObjects.Expressions;
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

		public abstract ResolvedExpression ChangeType(TypeSpecifier type);
		public abstract void WriteToRegisters(int[] regs, IRBuilder builder);
		public abstract void WriteToPointer(IPointer pointer, IRBuilder builder);

		public virtual int[] GetOrWriteToRegisters(int[] regs, IRBuilder builder)
		{
			WriteToRegisters(regs, builder);
			return regs;
		}

		public ResolvedExpression Cast(TypeSpecifier type)
		{
			if (Type == type) return this;
			else if (Type.PointerType != PointerType.None || type.PointerType != PointerType.None) throw new NotImplementedException();
			else if (Type is BuiltinTypeSpecifier actual && type is BuiltinTypeSpecifier expected)
			{
				if (actual.Size == expected.Size) return ChangeType(expected);
				return AsCasted(expected);
			}

			throw new InvalidOperationException($"Cannot cast {Type} to {type}");
		}

		protected virtual ResolvedExpression AsCasted(TypeSpecifier type) => new CastExpression(this, type);
	}

	public abstract class UnresolvedExpression : IExpression
	{
		public TypeSpecifier? GetTypeSpecifier() => null;
		public abstract ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope);
	}
}
