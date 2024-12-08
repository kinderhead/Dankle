using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public enum ArithmeticOperation
	{
		Addition,
		Multiplication
	}

	public class ArithmeticExpression(IExpression left, ArithmeticOperation op, IExpression right) : UnresolvedExpression
	{
		public readonly IExpression Left = left;
		public readonly ArithmeticOperation Op = op;
		public readonly IExpression Right = right;

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var left = Left.Resolve(builder, func, scope);
			var right = Right.Resolve(builder, func, scope);
			if (!left.Type.IsNumber() || !right.Type.IsNumber()) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

			var largest = left.Type.Size >= right.Type.Size ? left.Type : right.Type;
			var smallest = left.Type.Size >= right.Type.Size ? right.Type : left.Type;

			TypeSpecifier type;
			if (left.Type == right.Type) type = left.Type;
			else if (largest.IsSigned() == smallest.IsSigned()) type = largest;
			else if (largest.Size > smallest.Size) type = largest;
			else
			{
				if (largest.IsSigned()) type = smallest;
				else type = largest;
			}

			return new ResolvedArithmeticExpression(builder.Cast(left, type), Op, builder.Cast(right, type), type);
		}
	}

	public class ResolvedArithmeticExpression(ResolvedExpression left, ArithmeticOperation op, ResolvedExpression right, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly ResolvedExpression Left = left;
		public readonly ArithmeticOperation Op = op;
		public readonly ResolvedExpression Right = right;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedArithmeticExpression(Left, Op, Right, type);

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			var leftregs = Left.GetOrWriteToRegisters(regs, builder);
			var rightregs = Right.GetOrWriteToRegisters(IRBuilder.FitTempRegs(Type.Size), builder);

			if (Type.Size > 4) throw new NotImplementedException();

			if (Type.Size <= 2)
			{
				switch (Op)
				{
					case ArithmeticOperation.Addition:
						builder.Add(new AddRegs(leftregs[0], rightregs[0], regs[0]));
						break;
					case ArithmeticOperation.Multiplication:
						if (Type.IsSigned()) builder.Add(new SMulRegs(leftregs[0], rightregs[0], regs[0]));
						else builder.Add(new UMulRegs(leftregs[0], rightregs[0], regs[0]));
						break;
					default:
						break;
				}
			}
			else if (Type.Size <= 4)
			{
				switch (Op)
				{
					case ArithmeticOperation.Addition:
						builder.Add(new AddRegs(leftregs[1], rightregs[1], regs[1]));
						builder.Add(new AdcRegs(leftregs[0], rightregs[0], regs[0]));
						break;
					case ArithmeticOperation.Multiplication:
						throw new NotImplementedException();
					default:
						break;
				}
			}
		}
	}
}
