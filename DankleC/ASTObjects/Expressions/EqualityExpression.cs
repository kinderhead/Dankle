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

        public override ResolvedExpression Resolve(IRBuilder builder)
		{
            var left = Left.Resolve(builder);
			var right = Right.Resolve(builder);
			if (!left.Type.IsNumber() || !right.Type.IsNumber()) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

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

                return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), res ? 1 : 0);
            }

            //if (type.Size == 1 && type.IsSigned() && Op != EqualityOperation.Equals && Op != EqualityOperation.NotEquals) type = new BuiltinTypeSpecifier(BuiltinType.SignedShort);

            left = left.Cast(type);
            right = right.Cast(type);

            //if (type.Size == 4 && (Op == EqualityOperation.Equals || Op == EqualityOperation.NotEquals)) return new 

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

        public override IValue Execute(IRBuilder builder)
		{
            Compare(builder, Op, true);
            return ReturnValue(builder);
        }

        public override void Conditional(IRBuilder builder, bool negate = false)
		{
            Compare(builder, negate ? Invert(Op) : Op, false);
        }

        private void Compare(IRBuilder builder, EqualityOperation op, bool ret)
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

            switch (op)
            {
                case EqualityOperation.Equals:
                    builder.Add(new IREq(left, right, ret));
                    break;
                case EqualityOperation.NotEquals:
                    builder.Add(new IRNeq(left, right, ret));
                    break;
                case EqualityOperation.LessThan:
                    builder.Add(new IRLt(left, right, ret));
                    break;
                case EqualityOperation.LessThanOrEqual:
                    builder.Add(new IRLte(left, right, ret));
                    break;
                case EqualityOperation.GreaterThan:
                    builder.Add(new IRGt(left, right, ret));
                    break;
                case EqualityOperation.GreaterThanOrEqual:
                    builder.Add(new IRGte(left, right, ret));
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            save?.Dispose();
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

		public override void Walk(Action<ResolvedExpression> cb)
		{
            cb(this);
            Left.Walk(cb);
            Right.Walk(cb);
		}
	}
}
