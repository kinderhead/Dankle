using System;
using DankleC.ASTObjects.Expressions;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class SwitchStatement(IExpression expr, OrderedDictionary<Int128, ScopeNode> cases, ScopeNode? def) : Statement, IStatementHolder
    {
        public readonly IExpression Expression = expr;
        public readonly OrderedDictionary<Int128, ScopeNode> Cases = cases;
        public readonly ScopeNode? Default = def;

        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            var expr = Expression.Resolve(builder);
            var type = expr.Type;
            var val = builder.CurrentScope.AllocStackLocal("$switch", type);
            val.Store(builder, expr.Execute(builder));

            if (!type.IsNumber()) throw new InvalidOperationException("Switch can only be applied to number types");

            var labels = new OrderedDictionary<Int128, IRLogicLabel>();
            var def = new IRLogicLabel();
            var end = new IRLogicLabel();
            foreach (var i in Cases)
            {
                var label = new IRLogicLabel();
                labels[i.Key] = label;

                builder.Add(new IREq(val, new ConstantExpression(type, i.Key).Execute(builder), false));
                builder.Add(new IRJumpEq(label));
            }

            builder.Add(new IRJump(def));

            foreach (var i in Cases)
            {
                builder.Add(labels[i.Key]);
                builder.CurrentScope.SubScope(() => builder.ProcessStatement(i.Value, func, builder.CurrentScope), end);
            }

            builder.Add(def);
            if (Default is not null) builder.CurrentScope.SubScope(() => builder.ProcessStatement(Default, func, builder.CurrentScope), end);
            builder.Add(end);
        }

        public List<T> FindAll<T>() where T : Statement
        {
            var stmts = new List<T>();

            foreach (var i in Cases.Values)
            {
                if (i is T stmt) stmts.Add(stmt);
                stmts.AddRange(i.FindAll<T>());
            }

            if (Default is T stmt2) stmts.Add(stmt2);
            stmts.AddRange(Default?.FindAll<T>() ?? []);

            return stmts;
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
