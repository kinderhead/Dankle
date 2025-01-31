using System;

namespace DankleC.ASTObjects
{
    public class ProgramNode : IStatementHolder
    {
        public readonly List<FunctionNode> Functions = [];

        public List<T> FindAll<T>() where T : IStatement
        {
            List<T> stmts = [];

            foreach (var i in Functions)
            {
                stmts.AddRange(i.FindAll<T>());
            }

            return stmts;
        }
    }
}
