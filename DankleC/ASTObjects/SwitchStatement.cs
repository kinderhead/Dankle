using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class SwitchStatement(IExpression expr, Dictionary<Int128, ScopeNode> cases, ScopeNode? def) : Statement, IStatementHolder
    {
        public readonly IExpression Expression = expr;
        public readonly Dictionary<Int128, ScopeNode> Cases = cases;
        public readonly ScopeNode? Default = def;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            throw new NotImplementedException();
        }

        public List<T> FindAll<T>() where T : Statement
        {
            var stmts = new List<T>();
            
        }

        public void Optimize(ProgramNode.Settings settings)
        {
            foreach (var i in Cases.Values)
            {
                i.Optimize(settings);
            }

            Default?.Optimize(settings);
        }
    }
}
