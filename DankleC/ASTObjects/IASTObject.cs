using System;

namespace DankleC.ASTObjects
{
    public interface IASTObject
    {
        
    }

    public interface IStatementHolder : IASTObject
    {
        public List<T> FindAll<T>() where T : Statement;
    }
}
