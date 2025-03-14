using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public class CallExpression(IExpression func, ArgumentList args) : UnresolvedExpression
    {
        public readonly IExpression Function = func;
        public readonly ArgumentList Arguments = args;

        public override ResolvedExpression Resolve(IRBuilder builder) => new ResolvedCallExpression(Function.Resolve(builder), [.. Arguments.Arguments.Select(i => i.Resolve(builder))]);
    }

    public class ResolvedCallExpression(ResolvedExpression func, List<ResolvedExpression> args) : ResolvedExpression(((FunctionTypeSpecifier)func.Type).ReturnType)
    {
        public readonly ResolvedExpression Function = func;
        public readonly List<ResolvedExpression> Arguments = args;
        public override bool IsSimpleExpression => false;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedCallExpression(Function, Arguments);

        public override IValue Execute(IRBuilder builder)
		{
            var parameters = ((FunctionTypeSpecifier)Function.Type).Parameters;
            if (Arguments.Count != parameters.Parameters.Count) throw new InvalidOperationException("Mismatched argument count");

			builder.CurrentScope.ReserveFunctionCallSpace((FunctionTypeSpecifier)Function.Type);

            var reservedParams = 0;
			foreach (var i in Arguments)
			{
                i.Walk(expr =>
                {
                    if (expr is ResolvedCallExpression c) reservedParams = Math.Max(reservedParams, c.Arguments.Count - 1);
                });
			}

			var ptrs = new List<PreArgumentPointer>();
            var temps = new TempStackVariable?[Arguments.Count];

            var offset = 0;
            for (int i = 0; i < Arguments.Count; i++)
            {
                ptrs.Add(new PreArgumentPointer(offset, parameters.Parameters[i].Type.Size));
                offset += parameters.Parameters[i].Type.Size;
                if (i < reservedParams) temps[i] = builder.CurrentScope.AllocTemp(parameters.Parameters[i].Type);
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                builder.Add(new IRStorePtr(temps[i] is TempStackVariable v ? v.Pointer : ptrs[i], Arguments[i].Cast(parameters.Parameters[i].Type).Execute(builder)));
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (temps[i] is TempStackVariable v)
                {
                    if (i < reservedParams) builder.Add(new IRMovePointer(ptrs[i], v.Pointer));
                    builder.CurrentScope.FreeTemp(v);
                }
            }

            builder.Add(new IRCall(Function.Execute(builder)));

            if (Type.Size == 0) return new VoidValue();
            else return ReturnValue(builder);
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
            cb(this);
            Function.Walk(cb);

			foreach (var i in Arguments)
			{
                i.Walk(cb);
			}
		}
	}
}
