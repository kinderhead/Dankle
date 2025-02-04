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
		Subtraction,
		Multiplication,
		Division
	}

	public class ArithmeticExpression(IExpression left, ArithmeticOperation op, IExpression right) : UnresolvedExpression
	{
		public readonly IExpression Left = left;
		public readonly ArithmeticOperation Op = op;
		public readonly IExpression Right = right;

		public override void PrepScope(IRScope scope)
		{
			Left.PrepScope(scope);
			Right.PrepScope(scope);
		}

		public override ResolvedExpression Resolve(IRBuilder builder, IRFunction func, IRScope scope)
		{
			var left = Left.Resolve(builder, func, scope);
			var right = Right.Resolve(builder, func, scope);
			if (!left.Type.IsNumber() || !right.Type.IsNumber() || (left.Type is PointerTypeSpecifier && right.Type is PointerTypeSpecifier)) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

			TypeSpecifier type;
			if (left.Type is PointerTypeSpecifier lptr)
			{
				if (!(Op == ArithmeticOperation.Addition || Op == ArithmeticOperation.Subtraction)) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(left, Op, new ArithmeticExpression(right.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), lptr.Inner.Size)).Resolve(builder, func, scope), lptr);
			}
			else if (right.Type is PointerTypeSpecifier rptr)
			{
				if (Op != ArithmeticOperation.Addition) throw new InvalidOperationException("Invalid operation with pointer");
				return new ResolvedArithmeticExpression(new ArithmeticExpression(left.Cast(new BuiltinTypeSpecifier(BuiltinType.SignedInt)), ArithmeticOperation.Multiplication, new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), rptr.Inner.Size)).Resolve(builder, func, scope), Op, right, rptr);
			}
			else type = TypeSpecifier.GetOperationType(left.Type, right.Type);

			if (left is ConstantExpression l && right is ConstantExpression r)
			{
                dynamic res = Op switch
                {
                    ArithmeticOperation.Addition => (dynamic)l.Value + (dynamic)r.Value,
                    ArithmeticOperation.Subtraction => (dynamic)l.Value - (dynamic)r.Value,
					ArithmeticOperation.Multiplication => (dynamic)l.Value * (dynamic)r.Value,
					ArithmeticOperation.Division => (dynamic)l.Value / (dynamic)r.Value,
					_ => throw new NotImplementedException(),
                };
				
                return new ConstantExpression(type, res);
			}

			return new ResolvedArithmeticExpression(left.Cast(type), Op, right.Cast(type), type);
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

			using var tmp = builder.CurrentScope.AllocTempRegs(Type.Size, leftregs);
			var rightregs = Right.GetOrWriteToRegisters(tmp.Registers, builder);
			Compute(leftregs, rightregs, regs, builder);
		}

		public void Compute(int[] leftregs, int[] rightregs, int[] output, IRBuilder builder)
		{
			if (Type.Size <= 2)
			{
				switch (Op)
				{
					case ArithmeticOperation.Addition:
						builder.Add(new AddRegs(leftregs[0], rightregs[0], output[0]));
						break;
					case ArithmeticOperation.Subtraction:
						builder.Add(new SubRegs(leftregs[0], rightregs[0], output[0]));
						break;
					case ArithmeticOperation.Multiplication:
						if (Type.IsSigned()) builder.Add(new SMulRegs(leftregs[0], rightregs[0], output[0]));
						else builder.Add(new UMulRegs(leftregs[0], rightregs[0], output[0]));
						break;
					case ArithmeticOperation.Division:
						if (Type.IsSigned())
						{
							if (Type.Size == 1)
							{
								builder.Add(new SignExtReg8(leftregs[0], leftregs[0]));
								builder.Add(new SignExtReg8(rightregs[0], rightregs[0]));
							}
							builder.Add(new SDivRegs(leftregs[0], rightregs[0], output[0]));
						}
						else builder.Add(new UDivRegs(leftregs[0], rightregs[0], output[0]));
						break;
					default:
						break;
				}
			}
			else if (Type.Size == 4)
			{
				switch (Op)
				{
					case ArithmeticOperation.Addition:
						builder.Add(new AddRegs(leftregs[1], rightregs[1], output[1]));
						builder.Add(new AdcRegs(leftregs[0], rightregs[0], output[0]));
						break;
					case ArithmeticOperation.Subtraction:
						builder.Add(new SubRegs(leftregs[1], rightregs[1], output[1]));
						builder.Add(new SbbRegs(leftregs[0], rightregs[0], output[0]));
						break;
					case ArithmeticOperation.Multiplication:
						if (Type.IsSigned()) builder.Add(new SMul32Regs(leftregs[0], leftregs[1], rightregs[0], rightregs[1], output[0], output[1]));
						else builder.Add(new UMul32Regs(leftregs[0], leftregs[1], rightregs[0], rightregs[1], output[0], output[1]));
						break;
					case ArithmeticOperation.Division:
						if (Type.IsSigned()) builder.Add(new SDiv32Regs(leftregs[0], leftregs[1], rightregs[0], rightregs[1], output[0], output[1]));
						else builder.Add(new UDiv32Regs(leftregs[0], leftregs[1], rightregs[0], rightregs[1], output[0], output[1]));
						break;
					default:
						break;
				}
			}
			else throw new NotImplementedException();
		}

		public override void WriteToPointer(IPointer pointer, IRBuilder builder, int[] usedRegs)
		{
			if (Left.Type.Size == 1)
			{
				using var tmp = builder.CurrentScope.AllocTempRegs(4, usedRegs);
				Compute(Left.GetOrWriteToRegisters([tmp.Registers[0]], builder), Right.GetOrWriteToRegisters([tmp.Registers[1]], builder), [tmp.Registers[0]], builder);
				builder.Add(new LoadRegToPtr8(pointer, tmp.Registers[0]));
			}
			else if (Left.Type.Size == 2)
			{
				using var tmp = builder.CurrentScope.AllocTempRegs(4, usedRegs);
				Compute(Left.GetOrWriteToRegisters([tmp.Registers[0]], builder), Right.GetOrWriteToRegisters([tmp.Registers[1]], builder), [tmp.Registers[0]], builder);
				builder.Add(new LoadRegToPtr(pointer, tmp.Registers[0]));
			}
			else if (Left.Type.Size == 4)
			{
				var tmp1 = builder.CurrentScope.AllocTempRegs(4, usedRegs);
				var regs1 = Left.GetOrWriteToRegisters([tmp1.Registers[0], tmp1.Registers[1]], builder);
				var tmp2 = builder.CurrentScope.AllocTempRegs(4, [..tmp1.Registers, ..usedRegs]);
				var regs2 = Right.GetOrWriteToRegisters([tmp2.Registers[0], tmp2.Registers[1]], builder);
				Compute(regs1, regs2, [tmp1.Registers[0], tmp1.Registers[1]], builder);
				builder.Add(new LoadRegToPtr(pointer, tmp1.Registers[0]));
				builder.Add(new LoadRegToPtr(pointer.Get(2), tmp1.Registers[1]));
				tmp2.Dispose();
				tmp1.Dispose();
			}
			else throw new NotImplementedException();
		}

		public override void PrepScope(IRScope scope)
		{
			Left.PrepScope(scope);
			Right.PrepScope(scope);
		}
	}
}
