using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public enum EqualityOperation
    {
        Equals,
        NotEquals,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    public class EqualityExpression(IExpression left, EqualityOperation op, IExpression right) : UnresolvedExpression
    {
        public readonly IExpression Left = left;
		public readonly EqualityOperation Op = op;
		public readonly IExpression Right = right;

        public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
        {
            var left = Left.Resolve(builder, func, scope);
			var right = Right.Resolve(builder, func, scope);
			if (!left.Type.IsNumber() || !right.Type.IsNumber() || left.Type is PointerTypeSpecifier || right.Type is PointerTypeSpecifier) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

            var type = TypeSpecifier.GetOperationType(left.Type, right.Type);

            if (left is ConstantExpression l && right is ConstantExpression r)
            {
                dynamic res = Op switch
                {
                    EqualityOperation.Equals => (dynamic)l.Value == (dynamic)r.Value,
                    EqualityOperation.NotEquals => (dynamic)l.Value == (dynamic)r.Value,
                    EqualityOperation.LessThan => (dynamic)l.Value < (dynamic)r.Value,
                    EqualityOperation.LessThanOrEqual => (dynamic)l.Value <= (dynamic)r.Value,
                    EqualityOperation.GreaterThan => (dynamic)l.Value > (dynamic)r.Value,
                    EqualityOperation.GreaterThanOrEqual => (dynamic)l.Value >= (dynamic)r.Value,
					_ => throw new NotImplementedException(),
                };

                return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), res);
            }

            //if (type.Size == 1 && type.IsSigned() && Op != EqualityOperation.Equals && Op != EqualityOperation.NotEquals) type = new BuiltinTypeSpecifier(BuiltinType.SignedShort);

            left = left.Cast(type);
            right = right.Cast(type);

            return new ResolvedEqualityExpression(left, Op, right, type);
        }
    }

    public class ResolvedEqualityExpression(ResolvedExpression left, EqualityOperation op, ResolvedExpression right, TypeSpecifier type) : ResolvedExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar))
    {
        public readonly ResolvedExpression Left = left;
        public readonly EqualityOperation Op = op;
        public readonly ResolvedExpression Right = right;
        public readonly TypeSpecifier SourceType = type;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedEqualityExpression(Left, Op, Right, SourceType);

        public override IValue Execute(IRBuilder builder, IRScope scope)
        {
            Compare(builder, scope, Op, true);
            return ReturnValue();
        }

        public override void Conditional(IRBuilder builder, IRScope scope, Action ifTrue, Action? ifFalse)
        {
            var trueLabel = new IRLogicLabel();
            var falseLabel = new IRLogicLabel();

            Compare(builder, scope, Op, false);
            if (Op == EqualityOperation.NotEquals) builder.Add(new IRJumpEq(falseLabel));
            else builder.Add(new IRJumpNeq(falseLabel));

            ifTrue();

            if (ifFalse is not null) builder.Add(new IRJump(trueLabel));

            builder.Add(falseLabel);

            if (ifFalse is not null)
            {
                ifFalse();
                builder.Add(trueLabel);
            }
        }

        private void Compare(IRBuilder builder, IRScope scope, EqualityOperation op, bool ret)
        {
            switch (op)
            {
                case EqualityOperation.Equals:
                    builder.Add(new IREq(Left.Execute(builder, scope), Right.Execute(builder, scope), ret));
                    break;
                case EqualityOperation.NotEquals:
                    builder.Add(new IRNeq(Left.Execute(builder, scope), Right.Execute(builder, scope), ret));
                    break;
                case EqualityOperation.LessThan:
                    builder.Add(new IRLt(Left.Execute(builder, scope), Right.Execute(builder, scope), ret));
                    break;
                case EqualityOperation.LessThanOrEqual:
                    builder.Add(new IRLte(Left.Execute(builder, scope), Right.Execute(builder, scope), ret));
                    break;
                case EqualityOperation.GreaterThan:
                    builder.Add(new IRGt(Left.Execute(builder, scope), Right.Execute(builder, scope), ret));
                    break;
                case EqualityOperation.GreaterThanOrEqual:
                    builder.Add(new IRGte(Left.Execute(builder, scope), Right.Execute(builder, scope), ret));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static EqualityOperation Invert(EqualityOperation op) => op switch
        {
            EqualityOperation.Equals => EqualityOperation.NotEquals,
            EqualityOperation.NotEquals => EqualityOperation.Equals,
            EqualityOperation.LessThan => EqualityOperation.GreaterThanOrEqual,
            EqualityOperation.LessThanOrEqual => EqualityOperation.GreaterThan,
            EqualityOperation.GreaterThan => EqualityOperation.LessThanOrEqual,
            EqualityOperation.GreaterThanOrEqual => EqualityOperation.LessThan,
            _ => op
        };
    }
}
