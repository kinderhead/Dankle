using DankleC.IR;
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
		And,

		LeftShift,
		RightShift,

		Null // Used by IRAssign
	}

	public class ArithmeticExpression(IExpression left, ArithmeticOperation op, IExpression right, bool obeyPointers = true) : UnresolvedExpression
	{
		public readonly IExpression Left = left;
		public readonly ArithmeticOperation Op = op;
		public readonly IExpression Right = right;
		public readonly bool ObeyPointers = obeyPointers;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			var left = Left.Resolve(builder);
			var right = Right.Resolve(builder);
			if (!left.Type.IsNumber() || !right.Type.IsNumber() || ((left.Type is PointerTypeSpecifier || left.Type is ArrayTypeSpecifier) && (right.Type is PointerTypeSpecifier || right.Type is ArrayTypeSpecifier))) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

			TypeSpecifier type;
			if (ObeyPointers && left.Type is PointerTypeSpecifier lptr)
			{
				if (!(Op == ArithmeticOperation.Addition || Op == ArithmeticOperation.Subtraction)) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(left, Op, new ArithmeticExpression(right.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), lptr.Inner.Size == 0 ? 1 : lptr.Inner.Size)).Resolve(builder), lptr);
			}
			else if (ObeyPointers && right.Type is PointerTypeSpecifier rptr)
			{
				if (Op != ArithmeticOperation.Addition) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(new ArithmeticExpression(left.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), rptr.Inner.Size == 0 ? 1 : rptr.Inner.Size)).Resolve(builder), Op, right, rptr);
			}
			else if (ObeyPointers && left.Type is ArrayTypeSpecifier larr)
			{
				if (!(Op == ArithmeticOperation.Addition || Op == ArithmeticOperation.Subtraction)) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(new ResolvedRefExpression((LValue)left), Op, new ArithmeticExpression(right.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), larr.Inner.Size)).Resolve(builder), larr.Inner.AsPointer());
			}
			else if (ObeyPointers && right.Type is ArrayTypeSpecifier rarr)
			{
				if (Op != ArithmeticOperation.Addition) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(new ArithmeticExpression(left.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), rarr.Inner.Size)).Resolve(builder), Op, new ResolvedRefExpression((LValue)right), rarr.Inner.AsPointer());
			}
			else type = TypeSpecifier.GetOperationType(left.Type, right.Type);

			if (type.Size == 1 && type.IsSigned()) type = new BuiltinTypeSpecifier(BuiltinType.SignedShort);
			else if (type.Size == 1 && !type.IsSigned()) type = new BuiltinTypeSpecifier(BuiltinType.UnsignedShort);

			left = left.Cast(type);
			if (Op == ArithmeticOperation.LeftShift || Op == ArithmeticOperation.RightShift) right = right.Cast(new BuiltinTypeSpecifier(BuiltinType.UnsignedShort));
			else right = right.Cast(type);

			if (left is ConstantExpression l && right is ConstantExpression r)
			{
                dynamic res = Op switch
                {
                    ArithmeticOperation.Addition => (Int128)(dynamic)l.Value + (dynamic)r.Value, // Silly longs
                    ArithmeticOperation.Subtraction => (Int128)(dynamic)l.Value - (dynamic)r.Value,
					ArithmeticOperation.Multiplication => (Int128)(dynamic)l.Value * (dynamic)r.Value,
					ArithmeticOperation.Division => (Int128)(dynamic)l.Value / (dynamic)r.Value,
					ArithmeticOperation.Modulo => (Int128)(dynamic)l.Value % (dynamic)r.Value,
					ArithmeticOperation.InclusiveOr => (Int128)(dynamic)l.Value | (dynamic)r.Value,
					ArithmeticOperation.ExclusiveOr => (Int128)(dynamic)l.Value ^ (dynamic)r.Value,
					ArithmeticOperation.And => (Int128)(dynamic)l.Value & (dynamic)r.Value,
					ArithmeticOperation.LeftShift => (Int128)(dynamic)l.Value << (dynamic)r.Value,
					ArithmeticOperation.RightShift => (Int128)(dynamic)l.Value >> (dynamic)r.Value,
					_ => throw new NotImplementedException(),
                };
				
                return new ConstantExpression(type, res);
			}

			return new ResolvedArithmeticExpression(left, Op, right, type);
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
				case ArithmeticOperation.LeftShift:
					builder.Add(new IRLeftShift(left, right));
					break;
				case ArithmeticOperation.RightShift:
					builder.Add(new IRRightShift(left, right));
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

			return ReturnValue(builder);
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
			Left.Walk(cb);
			Right.Walk(cb);
		}
	}
}
