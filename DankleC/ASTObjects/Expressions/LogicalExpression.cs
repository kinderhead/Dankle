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

        public override ResolvedExpression ChangeType(TypeSpecifier type)
        {
            throw new NotImplementedException();
        }

        public override IValue Execute(IRBuilder builder, IRScope scope)
        {
            throw new NotImplementedException();
        }
    }
}
