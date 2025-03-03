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
            if (Arguments.Count != ((FunctionTypeSpecifier)Function.Type).Parameters.Count) throw new InvalidOperationException("Mismatched argument count");

            scope.ReserveFunctionCallSpace(((FunctionTypeSpecifier)Function.Type).ReturnType, Arguments);

            var offset = 0;
            foreach (var i in Arguments)
            {
                builder.Add(new IRStorePtr(new PreArgumentPointer(offset, i.Type.Size), i.Execute(builder, scope)));
                offset += i.Type.Size;
            }

            builder.Add(new IRCall(Function.Execute(builder, scope)));
            return ReturnValue();
        }
    }
}
