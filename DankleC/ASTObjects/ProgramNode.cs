using System;
using DankleC.ASTObjects.Expressions;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class ProgramNode : IStatementHolder
    {
        public readonly List<IGlobalDefinition> Defs = [];
        public readonly Dictionary<string, TypeSpecifier> Externs = [];

        public List<T> FindAll<T>() where T : Statement
        {
            List<T> stmts = [];

            foreach (var i in Defs)
            {
                if (i is IStatementHolder holder) stmts.AddRange(holder.FindAll<T>());
            }

            return stmts;
        }

        public void Optimize(Settings settings)
        {
            foreach (var i in Defs)
            {
                if (i is IStatementHolder holder) holder.Optimize(settings);
            }
        }

        public readonly record struct Settings();
    }

    public readonly record struct GlobalVariableDecl(string Name, TypeSpecifier Type, IToBytes? Value) : IGlobalDefinition
    {
        public void Handle(IRBuilder builder)
        {
            builder.CurrentScope = new(null, builder, 0);
            var def = ((IToBytes?)Value?.Resolve(builder)?.Cast(Type))?.ToBytes(builder) ?? new Bytes(new byte[Type.Size]);
			var label = new IRLabel($"_{Name}");
			builder.StaticVariables[label.Name] = (label, def);
			builder.GlobalVariables[Name] = Type;
        }
    }
}
