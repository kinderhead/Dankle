﻿using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public enum ArithmeticOperation
	{
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Modulo,

		InclusiveOr,
		ExclusiveOr,
		And
	}

	public class ArithmeticExpression(IExpression left, ArithmeticOperation op, IExpression right) : UnresolvedExpression
	{
		public readonly IExpression Left = left;
		public readonly ArithmeticOperation Op = op;
		public readonly IExpression Right = right;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var left = Left.Resolve(builder);
			var right = Right.Resolve(builder);
			if (!left.Type.IsNumber() || !right.Type.IsNumber() || ((left.Type is PointerTypeSpecifier || left.Type is ArrayTypeSpecifier) && (right.Type is PointerTypeSpecifier || right.Type is ArrayTypeSpecifier))) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

			TypeSpecifier type;
			if (left.Type is PointerTypeSpecifier lptr)
			{
				if (!(Op == ArithmeticOperation.Addition || Op == ArithmeticOperation.Subtraction) || lptr.Inner.Size == 0) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(left, Op, new ArithmeticExpression(right.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), lptr.Inner.Size)).Resolve(builder), lptr);
			}
			else if (right.Type is PointerTypeSpecifier rptr)
			{
				if (Op != ArithmeticOperation.Addition || rptr.Inner.Size == 0) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(new ArithmeticExpression(left.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), rptr.Inner.Size)).Resolve(builder), Op, right, rptr);
			}
			else if (left.Type is ArrayTypeSpecifier larr)
			{
				if (!(Op == ArithmeticOperation.Addition || Op == ArithmeticOperation.Subtraction)) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(new ResolvedRefExpression((LValue)left), Op, new ArithmeticExpression(right.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), larr.Inner.Size)).Resolve(builder), larr.Inner.AsPointer());
			}
			else if (right.Type is ArrayTypeSpecifier rarr)
			{
				if (Op != ArithmeticOperation.Addition) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(new ArithmeticExpression(left.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), rarr.Inner.Size)).Resolve(builder), Op, new ResolvedRefExpression((LValue)right), rarr.Inner.AsPointer());
			}
			else type = TypeSpecifier.GetOperationType(left.Type, right.Type);

			if (left is ConstantExpression l && right is ConstantExpression r)
			{
                dynamic res = Op switch
                {
                    ArithmeticOperation.Addition => (dynamic)l.Value + (dynamic)r.Value,
                    ArithmeticOperation.Subtraction => (dynamic)l.Value - (dynamic)r.Value,
					ArithmeticOperation.Multiplication => (dynamic)l.Value * (dynamic)r.Value,
					ArithmeticOperation.Division => (dynamic)l.Value / (dynamic)r.Value,
					ArithmeticOperation.Modulo => (dynamic)l.Value % (dynamic)r.Value,
					ArithmeticOperation.InclusiveOr => (dynamic)l.Value | (dynamic)r.Value,
					ArithmeticOperation.ExclusiveOr => (dynamic)l.Value ^ (dynamic)r.Value,
					ArithmeticOperation.And => (dynamic)l.Value & (dynamic)r.Value,
					_ => throw new NotImplementedException(),
                };
				
                return new ConstantExpression(type, res);
			}

			if (type.Size == 1 && type.IsSigned()) type = new BuiltinTypeSpecifier(BuiltinType.SignedShort);
			else if (type.Size == 1 && !type.IsSigned()) type = new BuiltinTypeSpecifier(BuiltinType.UnsignedShort);

			return new ResolvedArithmeticExpression(left.Cast(type), Op, right.Cast(type), type);
		}
	}

	public class ResolvedArithmeticExpression(ResolvedExpression left, ArithmeticOperation op, ResolvedExpression right, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly ResolvedExpression Left = left;
		public readonly ArithmeticOperation Op = op;
		public readonly ResolvedExpression Right = right;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedArithmeticExpression(Left, Op, Right, type);

        public override IValue Execute(IRBuilder builder)
		{
			var left = Left.Execute(builder);
			TempStackVariable? save = null;

			if (left is SimpleRegisterValue && !Right.IsSimpleExpression)
			{
				save = builder.CurrentScope.AllocTemp(Type);
				save.Store(builder, left);
			}

			var right = Right.Execute(builder);

			if (save is not null) left = save;

			switch (Op)
			{
				case ArithmeticOperation.Addition:
					builder.Add(new IRAdd(left, right));
					break;
				case ArithmeticOperation.Subtraction:
					builder.Add(new IRSub(left, right));
					break;
				case ArithmeticOperation.Multiplication:
					builder.Add(new IRMul(left, right));
					break;
				case ArithmeticOperation.Division:
					builder.Add(new IRDiv(left, right));
					break;
				case ArithmeticOperation.Modulo:
					builder.Add(new IRMod(left, right));
					break;
				case ArithmeticOperation.InclusiveOr:
					builder.Add(new IRInclusiveOr(left, right));
					break;
				case ArithmeticOperation.ExclusiveOr:
					builder.Add(new IRExclusiveOr(left, right));
					break;
				case ArithmeticOperation.And:
					builder.Add(new IRAnd(left, right));
					break;
				default:
					throw new InvalidOperationException();
			}

			save?.Dispose();

			return ReturnValue();
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
			Left.Walk(cb);
			Right.Walk(cb);
		}
	}
}
