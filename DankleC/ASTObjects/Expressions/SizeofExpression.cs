using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class SizeofExpression(IExpression expr) : UnresolvedExpression
    {
        public readonly IExpression Expression = expr;

        public override ResolvedExpression Resolve(IRBuilder builder) => DankleCVisitor.GetSmallestConstantExpression(Expression.Resolve(builder).Type.Size);
    }
}
