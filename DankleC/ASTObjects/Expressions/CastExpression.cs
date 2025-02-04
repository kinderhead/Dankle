using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class CastExpression(IExpression expr, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly IExpression Expr = expr;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new CastExpression(Expr, type);

		public override void PrepScope(IRScope scope)
		{
			Expr.PrepScope(scope);
		}

		public override void WriteToPointer(IPointer pointer, IRBuilder builder, int[] usedRegs)
		{
			var expr = Expr.Resolve(builder, builder.CurrentFunction, builder.CurrentScope);

			if (Type.Size <= expr.Type.Size)
			{
				var regs = IRBuilder.FitTempRegs(Type.Size);
				expr.WriteToRegisters([.. Enumerable.Repeat(-1, IRBuilder.NumRegForBytes(expr.Type.Size) - IRBuilder.NumRegForBytes(Type.Size)), .. regs], builder);
				builder.MoveRegsToPtr(regs, pointer);
			}
			else
			{
				var padding = Type.Size - expr.Type.Size;
				expr.WriteToPointer(pointer.Get(padding), builder, usedRegs);

				if (expr.Type.IsSigned())
				{
					if (Type.Size == 4 && expr.Type.Size == 2) builder.Add(new SignExtPtr(pointer, pointer.Get(padding)));
					else if (Type.Size == 4 && expr.Type.Size == 1)
					{
						builder.Add(new SignExtPtr8(pointer.Get(2), pointer.Get(2)));
						builder.Add(new SignExtPtr(pointer, pointer.Get(2)));
					}
					else if (Type.Size == 2 && expr.Type.Size == 1) builder.Add(new SignExtPtr8(pointer, pointer));
					else throw new NotImplementedException();
				}
				else builder.Add(new Memset(pointer, padding, 0));
			}
		}

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			var expr = Expr.Resolve(builder, builder.CurrentFunction, builder.CurrentScope);

			if (Type.Size <= expr.Type.Size)
			{
				expr.WriteToRegisters([.. Enumerable.Repeat(-1, IRBuilder.NumRegForBytes(expr.Type.Size) - IRBuilder.NumRegForBytes(Type.Size)), .. regs], builder);
			}
			else
			{
				expr.WriteToRegisters(regs[(IRBuilder.NumRegForBytes(Type.Size) - IRBuilder.NumRegForBytes(expr.Type.Size))..], builder);

				if (expr.Type.IsSigned())
				{
					if (Type.Size == 4 && expr.Type.Size == 2) builder.Add(new SignExtReg(regs[0], regs[1]));
					else if (Type.Size == 4 && expr.Type.Size == 1)
					{
						builder.Add(new SignExtReg8(regs[1], regs[1]));
						builder.Add(new SignExtReg(regs[0], regs[1]));
					}
					else if (Type.Size == 2 && expr.Type.Size == 1) builder.Add(new SignExtReg8(regs[0], regs[0]));
					else throw new NotImplementedException();
				}
				else foreach (var i in regs[..(IRBuilder.NumRegForBytes(Type.Size) - IRBuilder.NumRegForBytes(expr.Type.Size))])
				{
					builder.Add(new LoadImmToReg(i, 0));
				}
			}
		}
	}
}
