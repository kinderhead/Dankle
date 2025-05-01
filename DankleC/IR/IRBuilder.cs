using DankleC.ASTObjects;
using DankleC.ASTObjects.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
#pragma warning disable CS8618
	public class IRBuilder
    {
        public readonly ProgramNode AST;
        public readonly bool Debug;

        public readonly List<IRFunction> Functions = [];
		public readonly Dictionary<string, (ILabel, IByteLike)> StaticVariables = [];
		public readonly Dictionary<string, TypeSpecifier> GlobalVariables = [];
		public Dictionary<string, TypeSpecifier> Externs = [];
		public HashSet<string> ExternsUsed = [];
		public readonly List<Literal> Literals = [];

		public IRFunction CurrentFunction { get; internal set; }
		public IRScope CurrentScope { get; internal set; }

		private int literalLabels = 0;

		public IRBuilder(ProgramNode ast, bool debug = false)
		{
			AST = ast;
			Debug = debug;
			CurrentScope = new(null, this, 0);
        }

        public void Build()
		{
			Externs = AST.Externs;

			foreach (var i in AST.Defs)
			{
				i.Handle(this);
			}
		}

		public void HandleScope(IRFunction func, IRScope scope)
		{
			CurrentScope = scope;
			scope.SetupArguments(func);
			scope.Start();
			ProcessStatements(scope.Scope?.Statements ?? throw new InvalidOperationException(), func, scope);
			// scope.End();
		}

		public void ProcessStatements(List<Statement> statements, IRFunction func, IRScope scope)
		{
			foreach (var i in statements)
			{
				ProcessStatement(i, func, scope);
			}
		}

		public void ProcessStatement(Statement statement, IRFunction func, IRScope scope)
		{
			statement.Scope = scope;
			if (Debug) Add(new IRLabel($"stmt_{statement.ID}"));
			statement.BuildIR(this, func);
		}

		public void Add(IRInsn insn)
		{
			insn.Scope = CurrentScope;
			CurrentFunction.Insns.Add(insn);
		}

		public IRLabel Add(Literal literal)
		{
			var label = new IRLabel($"I${literalLabels++}");
			literal.Label = label;
			Literals.Add(literal);
			return label;
		}

		// public static int[] FitTempRegs(int bytes)
		// {
		// 	var regs = NumRegForBytes(bytes);
		// 	if (regs == 1) return [8];
		// 	if (regs == 2) return [8, 9];
		// 	if (regs == 3) return [8, 9, 10];
		// 	if (regs == 4) return [8, 9, 10, 11];
		// 	throw new InvalidOperationException();
		// }

		public static int NumRegForBytes(int bytes) => (int)Math.Ceiling(bytes / 2.0);
	}
#pragma warning restore CS8618
}
