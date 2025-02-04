using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public enum EqualityOperation
    {
        Equals,
        NotEquals
    }

    public class EqualityExpression(IExpression left, EqualityOperation op, IExpression right) : UnresolvedExpression
    {
        public readonly IExpression Left = left;
		public readonly EqualityOperation Op = op;
		public readonly IExpression Right = right;

        public override void PrepScope(IRScope scope)
        {
            Left.PrepScope(scope);
			Right.PrepScope(scope);
        }

        public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
        {
            var left = Left.Resolve(builder, func, scope);
			var right = Right.Resolve(builder, func, scope);
			if (!left.Type.IsNumber() || !right.Type.IsNumber() || left.Type is PointerTypeSpecifier || right.Type is PointerTypeSpecifier) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

            var type = TypeSpecifier.GetOperationType(left.Type, right.Type);
            left = left.Cast(type);
            right = right.Cast(type);
        }
    }
}
