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

		public IRFunction CurrentFunction { get; private set; }
		public IRScope CurrentScope { get; private set; }

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

			CurrentFunction = func;

			HandleScope(func, new(node.Scope, this, 0));
			func.Insns.Add(new ReturnInsn());

			Functions.Add(func);
		}

		public void HandleScope(IRFunction func, IRScope scope)
		{
			CurrentScope = scope;
			foreach (var i in scope.Scope.Statements)
			{
				i.BuildIR(this, func, scope);
			}
			scope.End();
		}

		public void Add(IRInsn insn)
		{
			insn.Scope = CurrentScope;
			CurrentFunction.Insns.Add(insn);
		}

		public ResolvedExpression Cast(ResolvedExpression expr, TypeSpecifier type)
		{
			if (expr.Type == type) return expr;
			else if (expr.Type.PointerType != PointerType.None || type.PointerType != PointerType.None) { }
			else if (expr.Type is BuiltinTypeSpecifier actual && type is BuiltinTypeSpecifier expected)
			{
				if (actual.Size == expected.Size) return expr.ChangeType(expected);
				return expr.AsCasted(expected);
			}

			throw new InvalidOperationException($"Cannot cast {expr.Type} to {type}");
		}

		public static int[] FitTempRegs(int bytes)
		{
			var regs = NumRegForBytes(bytes);
			if (regs == 1) return [8];
			if (regs == 2) return [8, 9];
			if (regs == 3) return [8, 9, 10];
			if (regs == 4) return [8, 9, 10, 11];
			throw new NotImplementedException();
		}

		public static int NumRegForBytes(int bytes) => (int)Math.Ceiling(bytes / 2.0);
	}
#pragma warning restore CS8618
}
