using System;

namespace DankleC.ASTObjects
{
    public class ProgramNode : IStatementHolder
    {
        public readonly List<FunctionNode> Functions = [];

        public List<T> FindAll<T>() where T : Statement
        {
            List<T> stmts = [];

            foreach (var i in Functions)
            {
                stmts.AddRange(i.FindAll<T>());
            }

            return stmts;
        }

        public void Optimize(Settings settings)
        {
            foreach (var i in Functions)
            {
                i.Optimize(settings);
            }
        }

        public readonly record struct Settings();
    }
}
