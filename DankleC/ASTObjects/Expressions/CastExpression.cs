﻿using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class UnresolvedCastExpression(IExpression expr, TypeSpecifier type) : UnresolvedExpression
	{
		public readonly IExpression Expr = expr;
		public readonly TypeSpecifier Type = type;

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => Expr.Resolve(builder, func, scope).Cast(Type);
	}

	public class CastExpression(ResolvedExpression expr, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly ResolvedExpression Expr = expr;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new CastExpression(Expr, type);

        public override IValue Execute(IRBuilder builder, IRScope scope)
		{
			builder.Add(new IRCast(Expr.Execute(builder, scope), Type));
			return ReturnValue();
        }
    }
}
