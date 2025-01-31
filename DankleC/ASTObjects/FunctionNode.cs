using System;

namespace DankleC.ASTObjects
{
    public class FunctionNode(string name, TypeSpecifier returnType, ScopeNode scope) : IStatementHolder
    {
        public readonly string Name = name;
        public readonly TypeSpecifier ReturnType = returnType;
        public readonly ScopeNode Scope = scope;

        public List<T> FindAll<T>() where T : IStatement => Scope.FindAll<T>();
    }
}
