using System;

namespace DankleC.ASTObjects
{
    public class FunctionNode(string name, FunctionTypeSpecifier type, ScopeNode scope) : IStatementHolder
    {
        public readonly string Name = name;
        public readonly FunctionTypeSpecifier Type = type;
        public readonly ScopeNode Scope = scope;

        public List<T> FindAll<T>() where T : Statement => Scope.FindAll<T>();

        public void Optimize(ProgramNode.Settings settings)
        {
            Scope.Optimize(settings);

            if (Scope.Statements.Count == 0 || Scope.Statements.Last() is not ReturnStatement)
            {
                if (Type.ReturnType != new BuiltinTypeSpecifier(BuiltinType.Void)) throw new InvalidOperationException($"Function \"{Name}\" missing return statement");
                else Scope.Statements.Add(new ReturnStatement(null));
            }
        }
    }
}
