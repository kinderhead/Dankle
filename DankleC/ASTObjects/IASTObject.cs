using System;

namespace DankleC.ASTObjects
{
    public interface IASTObject
    {

    }

    public interface IStatementHolder : IASTObject
    {
        public List<T> FindAll<T>() where T : Statement;
        public void Optimize(ProgramNode.Settings settings);
    }

    public record ParameterList(List<DeclaratorPair> Parameters) : IASTObject;
    public record ArgumentList(List<IExpression> Arguments) : IASTObject;
    public record DeclaratorPair(TypeSpecifier Type, string Name) : IASTObject;
}
