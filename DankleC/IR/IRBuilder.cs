using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
#pragma warning disable CS8618
	public class IRBuilder(ProgramNode ast)
	{
		public readonly ProgramNode AST = ast;

		public readonly List<IRFunction> Functions = [];

		private IRFunction currentFunction;
		private IRScope currentScope;

		public void Build()
		{
			foreach (var i in AST.Functions)
			{
				HandleFunction(i);
			}
		}

		private void HandleFunction(FunctionNode node)
		{
			var func = new IRFunction(node.Name, node.ReturnType);

			currentFunction = func;

			HandleScope(func, new(node.Scope));
			func.Insns.Add(new ReturnInsn());

			Functions.Add(func);
		}

		public void HandleScope(IRFunction func, IRScope scope)
		{
			currentScope = scope;
			foreach (var i in scope.Scope.Statements)
			{
				i.BuildIR(this, func, scope);
			}
		}

		public void Add(IRInsn insn) => currentFunction.Insns.Add(insn);

		public ResolvedExpression Cast(ResolvedExpression expr, TypeSpecifier type)
		{
			if (expr.Type == type) return expr;
			else if (expr.Type.PointerType != PointerType.None || type.PointerType != PointerType.None) { }
			else if (expr.Type is BuiltinTypeSpecifier actual && type is BuiltinTypeSpecifier expected)
			{
				if (actual.Size == expected.Size) return expr.ChangeType(expected);
				throw new NotImplementedException();
			}

			throw new InvalidOperationException($"Cannot cast {expr.Type} to {type}");
		}
	}
#pragma warning restore CS8618
}
