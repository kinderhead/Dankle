using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class WhileStatement(IExpression expr, Statement stmt, bool doWhile) : Statement, IStatementHolder
    {
        public readonly IExpression Expression = expr;
        public readonly Statement Statement = stmt;
        public readonly bool DoWhile = doWhile;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            var done = new IRLogicLabel();
            var loop = new IRLogicLabel();

            if (DoWhile)
            {
                builder.Add(loop);
                builder.ProcessStatement(Statement, func, Scope);
                Expression.Resolve(builder, func, Scope).Conditional(builder, Scope);
                builder.Add(new IRJumpEq(loop));
            }
            else
            {
                builder.Add(loop);
                Expression.Resolve(builder, func, Scope).Conditional(builder, Scope);
                builder.Add(new IRJumpNeq(done));
                builder.ProcessStatement(Statement, func, Scope);
                builder.Add(new IRJump(loop));
                builder.Add(done);
            }
        }

        public List<T> FindAll<T>() where T : Statement
        {
            List<T> stmts = [];

            if (Statement is T stmt) stmts.Add(stmt);
            if (Statement is IStatementHolder holder) stmts.AddRange(holder.FindAll<T>());

			return stmts;
        }

        public void Optimize(ProgramNode.Settings settings)
        {
            if (Statement is IStatementHolder holder) holder.Optimize(settings);
        }
    }
}
