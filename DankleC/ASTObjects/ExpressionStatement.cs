using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class ExpressionStatement(IExpression expr) : Statement
    {
        public readonly IExpression Expression = expr;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            Expression.Resolve(builder, func, Scope).Standalone().Execute(builder, Scope);
        }
    }
}
