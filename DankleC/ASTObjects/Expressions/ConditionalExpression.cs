using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class ConditionalExpression(IExpression cond, IExpression t, IExpression f) : UnresolvedExpression
    {
        public readonly IExpression Condition = cond;
        public readonly IExpression True = t;
        public readonly IExpression False = f;

        public override ResolvedExpression Resolve(IRBuilder builder)
        {
            var cond = Condition.Resolve(builder);
            var t = True.Resolve(builder);
            var f = False.Resolve(builder);

            TypeSpecifier type;

            if (t.Type == f.Type) type = t.Type;
            else if (t.Type.IsNumber() && f.Type.IsNumber()) type = TypeSpecifier.GetOperationType(t.Type, f.Type);
            else throw new InvalidOperationException();

            return new ResolvedConditionalExpression(cond, t.Cast(type), f.Cast(type), type);
        }
    }

    public class ResolvedConditionalExpression(ResolvedExpression cond, ResolvedExpression t, ResolvedExpression f, TypeSpecifier type) : ResolvedExpression(type)
    {
        public readonly ResolvedExpression Condition = cond;
        public readonly ResolvedExpression True = t;
        public readonly ResolvedExpression False = f;

        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedConditionalExpression(Condition, True, False, type);

        public override IValue Execute(IRBuilder builder)
        {
            if (Condition is ConstantExpression c)
            {
                if (c.IsTrue) return True.Execute(builder);
                else return False.Execute(builder);
            }
            else
            {
                var falseLabel = new IRLogicLabel();
                var trueLabel = new IRLogicLabel();

                Condition.Conditional(builder);
                builder.Add(new IRJumpNeq(falseLabel));
                True.Execute(builder);
                builder.Add(new IRJump(trueLabel));
                builder.Add(falseLabel);
                False.Execute(builder);
                builder.Add(trueLabel);

                return ReturnValue(builder);
            }
        }

        public override void Walk(Action<ResolvedExpression> cb)
        {
            cb(this);
            Condition.Walk(cb);
            True.Walk(cb);
            False.Walk(cb);
        }
    }
}
