using DankleC.ASTObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
#pragma warning disable CS8618
	public class IRBuilder(ProgramNode ast, bool debug = false)
	{
		public readonly ProgramNode AST = ast;
		public readonly bool Debug = debug;

		public readonly List<IRFunction> Functions = [];

		public IRFunction CurrentFunction { get; private set; }
		public IRScope CurrentScope { get; private set; }

		private int logicLabelCounter = 0;

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
			scope.Start();
			foreach (var i in scope.Scope.Statements)
			{
				i.Scope = scope;
				i.PrepScope();
			}
			foreach (var i in scope.Scope.Statements)
			{
				if (Debug) Add(new IRLabel($"stmt_{i.ID}"));
				i.BuildIR(this, func);
			}
			scope.End();
		}

		public void Add(IRInsn insn)
		{
			insn.Scope = CurrentScope;
			CurrentFunction.Insns.Add(insn);
		}

		public IRLabel GetLogicLabel()
		{
			var name = $"L${logicLabelCounter++}";
			return new IRLabel(name);
		}

		public void MoveRegsToPtr(int[] regs, IPointer ptr)
		{
			if (regs.Length != NumRegForBytes(ptr.Size)) throw new InvalidOperationException();
			else if (ptr.Size == 1) Add(new LoadRegToPtr8(ptr, regs[0]));
			else if (ptr.Size % 2 == 0)
			{
				for (var i = 0; i < ptr.Size; i += 2)
				{
					Add(new LoadRegToPtr(ptr.Get(i), regs[i / 2]));
				}
			}
			else throw new InvalidOperationException();
		}

		public void MovePtrToRegs(IPointer ptr, int[] regs)
		{
			if (regs.Length != NumRegForBytes(ptr.Size)) throw new InvalidOperationException();
			else if (ptr.Size == 1) Add(new LoadPtrToReg8(regs[0], ptr));
			else if (ptr.Size % 2 == 0)
			{
				for (var i = 0; i < ptr.Size; i += 2)
				{
					Add(new LoadPtrToReg(regs[i / 2], ptr.Get(i)));
				}
			}
			else throw new InvalidOperationException();
		}

		public void MovePtrToPtr(IPointer src, IPointer dest)
		{
			if (src.Size > dest.Size) throw new InvalidOperationException();
			else
			{
				for (var i = 0; i < NumRegForBytes(src.Size); i++)
				{
					if ((i + 1) * 2 > src.Size)
					{
						Add(new LoadPtrToReg8(8, src.Get(i * 2)));
						Add(new LoadRegToPtr8(dest.Get(i * 2), 8));
					}
					else
					{
						Add(new LoadPtrToReg(8, src.Get(i * 2)));
						Add(new LoadRegToPtr(dest.Get(i * 2), 8));
					}
				}
			}
		}

		public static int[] FitTempRegs(int bytes)
		{
			var regs = NumRegForBytes(bytes);
			if (regs == 1) return [8];
			if (regs == 2) return [8, 9];
			if (regs == 3) return [8, 9, 10];
			if (regs == 4) return [8, 9, 10, 11];
			throw new InvalidOperationException();
		}

		public static int NumRegForBytes(int bytes) => (int)Math.Ceiling(bytes / 2.0);
	}
#pragma warning restore CS8618
}
