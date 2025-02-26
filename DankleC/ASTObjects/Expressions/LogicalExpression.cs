using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public enum LogicalOperation
    {
        And,
        Or
    }

    public class LogicalExpression(IExpression left, LogicalOperation op, IExpression right) : UnresolvedExpression
    {
        public readonly IExpression Left = left;
        public readonly LogicalOperation Op = op;
        public readonly IExpression Right = right;

        public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
        {
            var left = Left.Resolve(builder, func, scope);
            var right = Right.Resolve(builder, func, scope);
            if (!left.Type.IsNumber() || !right.Type.IsNumber() || left.Type is PointerTypeSpecifier || right.Type is PointerTypeSpecifier) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

            if (left is ConstantExpression l && right is ConstantExpression r)
            {
                dynamic res = Op switch
                {
                    LogicalOperation.And => (dynamic)l.Value && (dynamic)r.Value,
                    LogicalOperation.Or => (dynamic)l.Value || (dynamic)r.Value,
                    _ => throw new NotImplementedException(),
                };

                return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), res);
            }

            return new ResolvedLogicalExpression(left, Op, right);
        }
    }

    public class ResolvedLogicalExpression(ResolvedExpression left, LogicalOperation op, ResolvedExpression right) : ResolvedExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar))
    {
        public readonly ResolvedExpression Left = left;
        public readonly LogicalOperation Op = op;
        public readonly ResolvedExpression Right = right;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedLogicalExpression(Left, Op, Right);

        public override IValue Execute(IRBuilder builder, IRScope scope)
        {
            var trueLabel = new IRLogicLabel();
            var falseLabel = new IRLogicLabel();

            if (Op == LogicalOperation.And)
            {
                Left.Conditional(builder, scope, true);
                builder.Add(new IRJumpEq(falseLabel));
                Right.Conditional(builder, scope, true);
                builder.Add(new IRJumpEq(falseLabel));
                builder.Add(new IRSetReturn(new Immediate(1, BuiltinType.UnsignedChar)));
                builder.Add(new IRJump(trueLabel));
                builder.Add(falseLabel);
                builder.Add(new IRSetReturn(new Immediate(0, BuiltinType.UnsignedChar)));
                builder.Add(trueLabel);
            }
            else
            {
                Left.Conditional(builder, scope);
                builder.Add(new IRJumpEq(trueLabel));
                Right.Conditional(builder, scope);
                builder.Add(new IRJumpEq(trueLabel));
                builder.Add(new IRSetReturn(new Immediate(0, BuiltinType.UnsignedChar)));
                builder.Add(new IRJump(falseLabel));
                builder.Add(trueLabel);
                builder.Add(new IRSetReturn(new Immediate(1, BuiltinType.UnsignedChar)));
                builder.Add(falseLabel);
            }

            return ReturnValue();
        }
    }
}
