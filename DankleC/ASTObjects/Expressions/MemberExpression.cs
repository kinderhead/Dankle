﻿using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class MemberExpression(IExpression expr, string member) : UnresolvedLValue
	{
		public readonly IExpression Expression = expr;
		public readonly string Member = member;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var expr = Expression.Resolve(builder);
			var structType = (StructTypeSpecifier)expr.Type;
			return new ResolvedMemberExpression(expr, Member, structType.GetMember(Member));
		}
	}

	public class PointerMemberExpression(IExpression expr, string member) : UnresolvedLValue
	{
		public readonly IExpression Expression = expr;
		public readonly string Member = member;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var expr = Expression.Resolve(builder);

			return new MemberExpression(new DerefExpression(expr).Resolve(builder), Member).Resolve(builder);
		}
	}

	public class ResolvedMemberExpression(ResolvedExpression expr, string member, TypeSpecifier type) : LValue(type)
	{
		public readonly ResolvedExpression Expression = expr;
		public readonly string Member = member;

		public StructTypeSpecifier StructType => (StructTypeSpecifier)Expression.Type;

		public override bool IsSimpleExpression => false;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedMemberExpression(Expression, Member, type);

		public override IValue Execute(IRBuilder builder)
		{
			return new SimplePointerValue(GetPointer(builder), Type, builder.CurrentScope);
		}

		public override IValue GetRef(IRBuilder builder)
		{
			builder.Add(new IRLoadPtrAddress(GetPointer(builder)));
			return new SimpleRegisterValue(IRInsn.FitRetRegs(Type.AsPointer().Size), Type.AsPointer());
		}

		public override IPointer GetPointer(IRBuilder builder)
		{
			if (Expression is LValue l) return l.GetPointer(builder).Get(StructType.GetOffset(Member), Type.Size);
			else
			{
				if (Expression.Execute(builder) is IPointerValue val) return val.Pointer.Get(StructType.GetOffset(Member), Type.Size);
				else throw new InvalidOperationException();
			}
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
			Expression.Walk(cb);
		}

		public override void WriteFrom(IValue val, IRBuilder builder, int offset, int subTypeSize)
		{
			if (Expression is LValue l) l.WriteFrom(val, builder, StructType.GetOffset(Member) + offset, subTypeSize);
			else throw new InvalidOperationException();
		}
	}
}
