using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class ForStatement(Statement? stmt1, IExpression? cond, Statement? stmt3, Statement body) : Statement, IStatementHolder
    {
        public readonly Statement? Statement1 = stmt1;
        public readonly IExpression? Conditional = cond;
        public readonly Statement? Statement3 = stmt3;
        public readonly Statement Body = body;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            var done = new IRLogicLabel();
            var cont = new IRLogicLabel();
            var loop = new IRLogicLabel();

            Scope.SubScope(() =>
            {
                if (Statement1 is not null) builder.ProcessStatement(Statement1, func, Scope);

                builder.Add(loop);
                if (Conditional is not null)
                {
                    Conditional.Resolve(builder).Conditional(builder);
                    builder.Add(new IRJumpNeq(done));
                }

                builder.ProcessStatement(Body, func, Scope);
                builder.Add(cont);
                if (Statement3 is not null) builder.ProcessStatement(Statement3, func, Scope);

                builder.Add(new IRJump(loop));
                if (Conditional is not null) builder.Add(done);
            }, cont, done);
        }

        public List<T> FindAll<T>() where T : Statement
        {
            List<T> stmts = [];

            if (Statement1 is T stmt1) stmts.Add(stmt1);
            if (Statement1 is IStatementHolder holder1) stmts.AddRange(holder1.FindAll<T>());
            if (Statement3 is T stmt3) stmts.Add(stmt3);
            if (Statement3 is IStatementHolder holder3) stmts.AddRange(holder3.FindAll<T>());
            if (Body is T body) stmts.Add(body);
            if (Body is IStatementHolder holder) stmts.AddRange(holder.FindAll<T>());

			return stmts;
        }

        public void Optimize(ProgramNode.Settings settings)
        {
            if (Statement1 is IStatementHolder holder1) holder1.Optimize(settings);
            if (Statement3 is IStatementHolder holder3) holder3.Optimize(settings);
            if (Body is IStatementHolder holder) holder.Optimize(settings);
        }
    }
}
