using System;

namespace DankleC.ASTObjects
{
    public class FunctionNode(string name, TypeSpecifier returnType, ScopeNode scope) : IASTObject
    {
        public readonly string Name = name;
        public readonly TypeSpecifier ReturnType = returnType;
        public readonly ScopeNode Scope = scope;
    }
}
