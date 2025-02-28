using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DankleC.IR;

namespace DankleC.ASTObjects
{
	public class ScopeNode : Statement, IStatementHolder
	{
		public readonly List<Statement> Statements = [];

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
			Scope.SubScope(() => builder.ProcessStatements(Statements, func, Scope));
        }

        public List<T> FindAll<T>() where T : Statement
        {
			List<T> stmts = [];

			foreach (var i in Statements)
			{
				if (i is T stmt) stmts.Add(stmt);
				if (i is IStatementHolder holder) stmts.AddRange(holder.FindAll<T>());
			}

			return stmts;
        }
    }
}
