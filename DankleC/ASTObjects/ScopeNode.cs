using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public class ScopeNode : IASTObject, IStatementHolder
	{
		public readonly List<Statement> Statements = [];

        public List<T> FindAll<T>() where T : Statement
        {
			List<T> stmts = [];

			foreach (var i in Statements)
			{
				if (i is T stmt) stmts.Add(stmt);
				else if (i is IStatementHolder holder) stmts.AddRange(holder.FindAll<T>());
			}

			return stmts;
        }
    }
}
