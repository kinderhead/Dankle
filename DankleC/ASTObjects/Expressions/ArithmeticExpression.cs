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
		Division
	}

	public class ArithmeticExpression(IExpression left, ArithmeticOperation op, IExpression right) : UnresolvedExpression
	{
		public readonly IExpression Left = left;
		public readonly ArithmeticOperation Op = op;
		public readonly IExpression Right = right;

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var left = Left.Resolve(builder, func, scope);
			var right = Right.Resolve(builder, func, scope);
			if (!left.Type.IsNumber() || !right.Type.IsNumber() || (left.Type is PointerTypeSpecifier && right.Type is PointerTypeSpecifier)) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

			TypeSpecifier type;
			if (left.Type is PointerTypeSpecifier lptr)
			{
				if (!(Op == ArithmeticOperation.Addition || Op == ArithmeticOperation.Subtraction)) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(left, Op, new ArithmeticExpression(right.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), lptr.Inner.Size)).Resolve(builder, func, scope), lptr);
			}
			else if (right.Type is PointerTypeSpecifier rptr)
			{
				if (Op != ArithmeticOperation.Addition) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(new ArithmeticExpression(left.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), rptr.Inner.Size)).Resolve(builder, func, scope), Op, right, rptr);
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
					_ => throw new NotImplementedException(),
                };
				
                return new ConstantExpression(type, res);
			}

			return new ResolvedArithmeticExpression(left.Cast(type), Op, right.Cast(type), type);
		}
	}

	public class ResolvedArithmeticExpression(ResolvedExpression left, ArithmeticOperation op, ResolvedExpression right, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly ResolvedExpression Left = left;
		public readonly ArithmeticOperation Op = op;
		public readonly ResolvedExpression Right = right;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedArithmeticExpression(Left, Op, Right, type);

        public override IValue Execute(IRBuilder builder, IRScope scope)
		{
			var left = Left.Execute(builder, scope);
			TempStackVariable? save = null;

			if (left is SimpleRegisterValue)
			{
				save = scope.AllocTemp(Type);
				save.Store(builder, left);
			}

			var right = Right.Execute(builder, scope);

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
				default:
					throw new InvalidOperationException();
			}

			save?.Dispose();

			return ReturnValue();
		}
    }
}
