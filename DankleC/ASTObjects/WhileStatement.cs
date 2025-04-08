using System;
using DankleC.ASTObjects.Expressions;
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

            var expr = Expression.Resolve(builder);

            if (DoWhile)
            {
                var cont = new IRLogicLabel();

                builder.CurrentScope.SubScope(() =>
                {
                    builder.Add(loop);
                    builder.ProcessStatement(Statement, func, Scope);
                    builder.Add(cont);
                    expr.Conditional(builder);
                    builder.Add(new IRJumpEq(loop));
                    builder.Add(done);
                }, cont, done);
            }
            else
            {
                builder.CurrentScope.SubScope(() =>
                {
                    builder.Add(loop);
                    if (expr is not ConstantExpression c)
                    {
                        expr.Conditional(builder);
                        builder.Add(new IRJumpNeq(done));
                    }
                    builder.ProcessStatement(Statement, func, Scope);
                    builder.Add(new IRJump(loop));
                    builder.Add(done);
                }, loop, done);
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
