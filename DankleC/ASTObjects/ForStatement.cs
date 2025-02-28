using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class ForStatement(Statement? stmt1, Statement? stmt2, Statement? stmt3, Statement body) : Statement, IStatementHolder
    {
        public readonly Statement? Statement1 = stmt1;
        public readonly Statement? Statement2 = stmt2;
        public readonly Statement? Statement3 = stmt3;
        public readonly Statement Body = body;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            throw new NotImplementedException();
        }

        public List<T> FindAll<T>() where T : Statement
        {
            List<T> stmts = [];

            if (Statement1 is T stmt1) stmts.Add(stmt1);
            if (Statement1 is IStatementHolder holder1) stmts.AddRange(holder1.FindAll<T>());
            if (Statement2 is T stmt2) stmts.Add(stmt2);
            if (Statement2 is IStatementHolder holder2) stmts.AddRange(holder2.FindAll<T>());
            if (Statement3 is T stmt3) stmts.Add(stmt3);
            if (Statement3 is IStatementHolder holder3) stmts.AddRange(holder3.FindAll<T>());
            if (Body is T body) stmts.Add(body);
            if (Body is IStatementHolder holder) stmts.AddRange(holder.FindAll<T>());

			return stmts;
        }
    }
}
