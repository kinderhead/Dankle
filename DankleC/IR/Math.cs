using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using DankleC.ASTObjects;
using DankleC.ASTObjects.Expressions;
using System;

namespace DankleC.IR
{
	public static class IRMath
	{
		public static void Perform(IRInsn insn, IValue left, ArithmeticOperation op, IValue right, IValue ret)
		{
			switch (op)
			{
				case ArithmeticOperation.Addition:
					if (left.Type.Size <= 2) insn.Add(CGInsn.Build<Add>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
					else if (left.Type.Size == 4)
					{
						insn.Add(CGInsn.Build<Add>(left.MakeArg(1), right.MakeArg(1), ret.MakeArg(1)));
						insn.Add(CGInsn.Build<Adc>(left.MakeArg(0), right.MakeArg(0), ret.MakeArg(0)));
					}
					else if (left.Type.Size == 8)
					{
						insn.Add(CGInsn.Build<Add>(left.MakeArg(3), right.MakeArg(3), ret.MakeArg(3)));
						insn.Add(CGInsn.Build<Adc>(left.MakeArg(2), right.MakeArg(2), ret.MakeArg(2)));
						insn.Add(CGInsn.Build<Adc>(left.MakeArg(1), right.MakeArg(1), ret.MakeArg(1)));
						insn.Add(CGInsn.Build<Adc>(left.MakeArg(0), right.MakeArg(0), ret.MakeArg(0)));
					}
					else throw new NotImplementedException();
					break;
				case ArithmeticOperation.Subtraction:
					if (left.Type.Size <= 2) insn.Add(CGInsn.Build<Subtract>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
					else if (left.Type.Size == 4)
					{
						insn.Add(CGInsn.Build<Subtract>(left.MakeArg(1), right.MakeArg(1), ret.MakeArg(1)));
						insn.Add(CGInsn.Build<Sbb>(left.MakeArg(0), right.MakeArg(0), ret.MakeArg(0)));
					}
					else if (left.Type.Size == 8)
					{
						insn.Add(CGInsn.Build<Subtract>(left.MakeArg(3), right.MakeArg(3), ret.MakeArg(3)));
						insn.Add(CGInsn.Build<Sbb>(left.MakeArg(2), right.MakeArg(2), ret.MakeArg(2)));
						insn.Add(CGInsn.Build<Sbb>(left.MakeArg(1), right.MakeArg(1), ret.MakeArg(1)));
						insn.Add(CGInsn.Build<Sbb>(left.MakeArg(0), right.MakeArg(0), ret.MakeArg(0)));
					}
					else throw new NotImplementedException();
					break;
				case ArithmeticOperation.Multiplication:
					GenericPerform<SignedMul, SignedMul32, SignedMul64, UnsignedMul, UnsignedMul32, UnsignedMul64>(insn, left, right, ret);
					break;
				case ArithmeticOperation.Division:
					GenericPerform<SignedDiv, SignedDiv32, SignedDiv64, UnsignedDiv, UnsignedDiv32, UnsignedDiv64>(insn, left, right, ret);
					break;
				case ArithmeticOperation.Modulo:
					GenericPerform<SignedModulo, SignedModulo32, SignedModulo64, Modulo, Modulo32, Modulo64>(insn, left, right, ret);
					break;
				case ArithmeticOperation.LeftShift:
					GenericPerform<LeftShift, LeftShift32, LeftShift64, LeftShift, LeftShift32, LeftShift64>(insn, left, right, ret);
					break;
				case ArithmeticOperation.RightShift:
					GenericPerform<ArithmeticRightShift, ArithmeticRightShift32, ArithmeticRightShift64, RightShift, RightShift32, RightShift64>(insn, left, right, ret);
					break;
				case ArithmeticOperation.InclusiveOr:
					GenericBitwise<Or>(insn, left, right, ret);
					break;
				case ArithmeticOperation.ExclusiveOr:
					GenericBitwise<Xor>(insn, left, right, ret);
					break;
				case ArithmeticOperation.And:
					GenericBitwise<And>(insn, left, right, ret);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public static void GenericPerform<Op, Op32, Op64, UOp, UOp32, UOp64>(IRInsn insn, IValue left, IValue right, IValue ret) where Op : Instruction, new() where Op32 : Instruction, new() where Op64 : Instruction, new() where UOp : Instruction, new() where UOp32 : Instruction, new() where UOp64 : Instruction, new()
		{
			if (left.Type.IsSigned())
			{
				if (left.Type.Size == 1)
				{
					insn.Add(CGInsn.Build<SignExtend8>(left.MakeArg(), left.MakeArg()));
					insn.Add(CGInsn.Build<SignExtend8>(right.MakeArg(), right.MakeArg()));
				}

				if (left.Type.Size <= 2) insn.Add(CGInsn.Build<Op>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
				else if (left.Type.Size == 4) insn.Add(CGInsn.Build<Op32>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
				else if (left.Type.Size == 8) insn.Add(CGInsn.Build<Op64>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
			else
			{
				if (left.Type.Size <= 2) insn.Add(CGInsn.Build<UOp>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
				else if (left.Type.Size == 4) insn.Add(CGInsn.Build<UOp32>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
				else if (left.Type.Size == 8) insn.Add(CGInsn.Build<UOp64>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
		}

		public static void GenericBitwise<Op>(IRInsn insn, IValue left, IValue right, IValue ret) where Op : Instruction, new()
		{
			if (left.Type.Size == 1)
			{
				insn.Add(CGInsn.Build<Op>(left.MakeArg(), right.MakeArg(), ret.MakeArg()));
				return;
			}

			for (int i = left.Type.Size - 2; i >= 0; i -= 2)
			{
				insn.Add(CGInsn.Build<Op>(left.MakeArg(i / 2), right.MakeArg(i / 2), ret.MakeArg(i / 2)));
			}
		}
	}

	public class IRAdd(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			//if (Left.Type != Right.Type || !Left.Type.IsNumber() || !Right.Type.IsNumber()) throw new NotImplementedException();

			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.Addition, Right, ret);		
		}
	}

	public class IRSub(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			//if (Left.Type != Right.Type || !Left.Type.IsNumber() || !Right.Type.IsNumber()) throw new NotImplementedException();

			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.Subtraction, Right, ret);	
		}
	}

	public class IRMul(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			//if (Left.Type != Right.Type || !Left.Type.IsNumber() || !Right.Type.IsNumber()) throw new NotImplementedException();

			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.Multiplication, Right, ret);
		}
	}

	public class IRDiv(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			//if (Left.Type != Right.Type || !Left.Type.IsNumber() || !Right.Type.IsNumber()) throw new NotImplementedException();
			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.Division, Right, ret);	
		}
	}

	public class IRMod(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			//if (Left.Type != Right.Type || !Left.Type.IsNumber() || !Right.Type.IsNumber()) throw new NotImplementedException();
			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.Modulo, Right, ret);
		}
	}

	public class IRLeftShift(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.LeftShift, Right, ret);
		}
	}

	public class IRRightShift(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.RightShift, Right, ret);
		}
	}

	public class IRInclusiveOr(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.InclusiveOr, Right, ret);
		}
	}

	public class IRExclusiveOr(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.ExclusiveOr, Right, ret);
		}
	}

	public class IRAnd(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Left.Type);
			IRMath.Perform(this, Left, ArithmeticOperation.And, Right, ret);
		}
	}

	public class IRPostIncrement(IPointer ptr, TypeSpecifier type) : IRInsn
	{
		public readonly IPointer Pointer = ptr;
		public readonly TypeSpecifier Type = type;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Type);

			var ptr = Pointer;
			if (Pointer is RegisterPointer regptr && regptr.Reg1 == ret.Registers[0])
			{
				var newregs = Alloc(4);
				MoveRegsToRegs([regptr.Reg1, regptr.Reg2], newregs);
				ptr = new RegisterPointer(newregs[0], newregs[1], regptr.Offset, regptr.Size);
			}

			MovePtrToRegs(ptr, ret.Registers);

			var regs = new SimpleRegisterValue(Alloc(Type.Size), Type);
			if (Type.Size <= 2)
			{
				Add(CGInsn.Build<Add>(new CGRegister(FitRetRegs(Type.Size)[0]), new CGImmediate<ushort>(1), regs.MakeArg()));
			}
			else if (Type.Size == 4)
			{
				Add(CGInsn.Build<Add>(new CGRegister(FitRetRegs(Type.Size)[1]), new CGImmediate<ushort>(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(new CGRegister(FitRetRegs(Type.Size)[0]), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else if (Type.Size == 8)
			{
				Add(CGInsn.Build<Add>(new CGRegister(FitRetRegs(Type.Size)[3]), new CGImmediate<ushort>(1), regs.MakeArg(3)));
				Add(CGInsn.Build<Adc>(new CGRegister(FitRetRegs(Type.Size)[2]), new CGImmediate<ushort>(0), regs.MakeArg(2)));
				Add(CGInsn.Build<Adc>(new CGRegister(FitRetRegs(Type.Size)[1]), new CGImmediate<ushort>(0), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(new CGRegister(FitRetRegs(Type.Size)[0]), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			MoveRegsToPtr(regs.Registers, ptr);
		}
	}

	public class IRPostDecrement(IPointer ptr, TypeSpecifier type) : IRInsn
	{
		public readonly IPointer Pointer = ptr;
		public readonly TypeSpecifier Type = type;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Type);

			var ptr = Pointer;
			if (Pointer is RegisterPointer regptr && regptr.Reg1 == ret.Registers[0])
			{
				var newregs = Alloc(4);
				MoveRegsToRegs([regptr.Reg1, regptr.Reg2], newregs);
				ptr = new RegisterPointer(newregs[0], newregs[1], regptr.Offset, regptr.Size);
			}

			MovePtrToRegs(ptr, ret.Registers);

			var regs = new SimpleRegisterValue(Alloc(Type.Size), Type);
			if (Type.Size <= 2)
			{
				Add(CGInsn.Build<Subtract>(new CGRegister(FitRetRegs(Type.Size)[0]), new CGImmediate<ushort>(1), regs.MakeArg()));
			}
			else if (Type.Size == 4)
			{
				Add(CGInsn.Build<Subtract>(new CGRegister(FitRetRegs(Type.Size)[1]), new CGImmediate<ushort>(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(new CGRegister(FitRetRegs(Type.Size)[0]), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else if (Type.Size == 8)
			{
				Add(CGInsn.Build<Subtract>(new CGRegister(FitRetRegs(Type.Size)[3]), new CGImmediate<ushort>(1), regs.MakeArg(3)));
				Add(CGInsn.Build<Sbb>(new CGRegister(FitRetRegs(Type.Size)[2]), new CGImmediate<ushort>(0), regs.MakeArg(2)));
				Add(CGInsn.Build<Sbb>(new CGRegister(FitRetRegs(Type.Size)[1]), new CGImmediate<ushort>(0), regs.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(new CGRegister(FitRetRegs(Type.Size)[0]), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			MoveRegsToPtr(regs.Registers, ptr);
		}
	}

	public class IRPreIncrement(IPointer ptr, TypeSpecifier type) : IRInsn
	{
		public readonly IPointer Pointer = ptr;
		public readonly TypeSpecifier Type = type;

		public override void Compile(CodeGen gen)
		{
			var regs = new SimplePointerValue(Pointer, Type, Scope).ToRegisters(this);
			if (Type.Size <= 2)
			{
				Add(CGInsn.Build<Increment>(regs.MakeArg()));
			}
			else if (Type.Size == 4)
			{
				Add(CGInsn.Build<Add>(regs.MakeArg(1), new CGImmediate<ushort>(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(0), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else if (Type.Size == 8)
			{
				Add(CGInsn.Build<Add>(regs.MakeArg(3), new CGImmediate<ushort>(1), regs.MakeArg(3)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(2), new CGImmediate<ushort>(0), regs.MakeArg(2)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(1), new CGImmediate<ushort>(0), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(0), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			MoveRegsToPtr(regs.Registers, Pointer);
			Return(regs);
		}
	}

	public class IRPreDecrement(IPointer ptr, TypeSpecifier type) : IRInsn
	{
		public readonly IPointer Pointer = ptr;
		public readonly TypeSpecifier Type = type;

		public override void Compile(CodeGen gen)
		{
			var regs = new SimplePointerValue(Pointer, Type, Scope).ToRegisters(this);
			if (Type.Size <= 2)
			{
				Add(CGInsn.Build<Decrement>(regs.MakeArg()));
			}
			else if (Type.Size == 4)
			{
				Add(CGInsn.Build<Subtract>(regs.MakeArg(1), new CGImmediate<ushort>(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(regs.MakeArg(0), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else if (Type.Size == 8)
			{
				Add(CGInsn.Build<Subtract>(regs.MakeArg(3), new CGImmediate<ushort>(1), regs.MakeArg(3)));
				Add(CGInsn.Build<Sbb>(regs.MakeArg(2), new CGImmediate<ushort>(0), regs.MakeArg(2)));
				Add(CGInsn.Build<Sbb>(regs.MakeArg(1), new CGImmediate<ushort>(0), regs.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(regs.MakeArg(0), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			MoveRegsToPtr(regs.Registers, Pointer);
			Return(regs);
		}
	}
	
	public class IRNegate(IValue val) : IRInsn
	{
		public readonly IValue Value = val;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Value.Type);

			for (int i = 0; i < IRBuilder.NumRegForBytes(Value.Type.Size); i++)
			{
				Add(CGInsn.Build<Xor>(Value.MakeArg(i), new CGImmediate<ushort>(0xFFFF), ret.MakeArg(i)));
			}
		}
	}
}
