using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class CallExpression(IExpression func, ArgumentList args) : UnresolvedExpression
    {
        public readonly IExpression Function = func;
        public readonly ArgumentList Arguments = args;

        public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope) => new ResolvedCallExpression(Function.Resolve(builder, func, scope), [.. Arguments.Arguments.Select(i => i.Resolve(builder, func, scope))]);
    }

    public class ResolvedCallExpression(ResolvedExpression func, List<ResolvedExpression> args) : ResolvedExpression(((FunctionTypeSpecifier)func.Type).ReturnType)
    {
        public readonly ResolvedExpression Function = func;
        public readonly List<ResolvedExpression> Arguments = args;
        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedCallExpression(Function, Arguments);

        public override IValue Execute(IRBuilder builder, IRScope scope)
        {
            var parameters = ((FunctionTypeSpecifier)Function.Type).Parameters;
            if (Arguments.Count != parameters.Count) throw new InvalidOperationException("Mismatched argument count");

            scope.ReserveFunctionCallSpace((FunctionTypeSpecifier)Function.Type);

            var ptrs = new List<PreArgumentPointer>();
            var temps = new TempStackVariable[Arguments.Count];

            var offset = 0;
            for (int i = 0; i < Arguments.Count; i++)
            {
                ptrs.Add(new PreArgumentPointer(offset, parameters[i].Size));
                offset += parameters[i].Size;
                temps[i] = scope.AllocTemp(parameters[i]);
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                builder.Add(new IRStorePtr(temps[i].Pointer, Arguments[i].Cast(parameters[i]).Execute(builder, scope)));
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (temps[i] is TempStackVariable v)
                {
                    builder.Add(new IRMovePointer(ptrs[i], v.Pointer));
                    scope.FreeTemp(v);
                }
            }

            builder.Add(new IRCall(Function.Execute(builder, scope)));
            return ReturnValue();
        }
    }
}
