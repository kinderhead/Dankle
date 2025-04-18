﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DankleC.IR;

namespace DankleC.ASTObjects
{
	public class ScopeNode(bool subScope = true) : Statement, IStatementHolder
	{
		public readonly List<Statement> Statements = [];
		public readonly bool SubScope = subScope;

		public override void BuildIR(IRBuilder builder, IRFunction func)
		{
			if (SubScope) Scope.SubScope(() => builder.ProcessStatements(Statements, func, Scope));
			else builder.ProcessStatements(Statements, func, Scope);
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

        public void Optimize(ProgramNode.Settings settings)
        {
			for (int i = 0; i < Statements.Count; i++)
			{
				if (Statements[i] is ReturnStatement)
				{
					Statements.RemoveRange(i + 1, Statements.Count - i - 1);
					break;
				}
			}

            foreach (var stmt in Statements)
			{
				if (stmt is IStatementHolder holder) holder.Optimize(settings);
			}
        }
    }
}
