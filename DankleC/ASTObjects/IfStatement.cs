using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class IfStatement(IExpression conditional, Statement stmt) : Statement, IStatementHolder
    {
        public readonly IExpression Conditional = conditional;
        public readonly Statement Statement = stmt;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            var falseLabel = new IRLogicLabel();
            Conditional.Resolve(builder, func, Scope).Conditional(builder, Scope);
            builder.Add(new IRJumpNeq(falseLabel));
            builder.ProcessStatement(Statement, func, Scope);
            builder.Add(falseLabel);
        }

        public List<T> FindAll<T>() where T : Statement
        {
            List<T> stmts = [];

            if (Statement is T stmt) stmts.Add(stmt);
            else if (Statement is IStatementHolder holder) stmts.AddRange(holder.FindAll<T>());

			return stmts;
        }
    }
}
