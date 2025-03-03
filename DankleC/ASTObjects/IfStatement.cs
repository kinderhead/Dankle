using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class IfStatement(IExpression conditional, Statement stmt, Statement? elseStmt) : Statement, IStatementHolder
    {
        public readonly IExpression Conditional = conditional;
        public readonly Statement Statement = stmt;
        public readonly Statement? Else = elseStmt;

		public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            var falseLabel = new IRLogicLabel();
            var trueLabel = new IRLogicLabel();

            Conditional.Resolve(builder, func, Scope).Conditional(builder, Scope);
            builder.Add(new IRJumpNeq(falseLabel));
            builder.ProcessStatement(Statement, func, Scope);
            if (Else is not null) builder.Add(new IRJump(trueLabel));
            builder.Add(falseLabel);
            if (Else is not null)
            {
                builder.ProcessStatement(Else, func, Scope);
				builder.Add(trueLabel);
			}
		}

        public List<T> FindAll<T>() where T : Statement
        {
            List<T> stmts = [];

            if (Statement is T stmt) stmts.Add(stmt);
            if (Statement is IStatementHolder holder) stmts.AddRange(holder.FindAll<T>());

			if (Else is T estmt) stmts.Add(estmt);
			if (Else is IStatementHolder eholder) stmts.AddRange(eholder.FindAll<T>());

			return stmts;
        }

        public void Optimize(ProgramNode.Settings settings)
        {
            if (Statement is IStatementHolder holder) holder.Optimize(settings);
            if (Else is IStatementHolder eholder) eholder.Optimize(settings);
        }
    }
}
