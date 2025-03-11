using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using System;

namespace DankleC.IR
{
	public class IRAdd(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			//if (Left.Type != Right.Type || !Left.Type.IsNumber() || !Right.Type.IsNumber()) throw new NotImplementedException();

			var ret = GetReturn(Left.Type);

			if (Left.Type.Size <= 2) Add(CGInsn.Build<Add>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
			else if (Left.Type.Size == 4)
			{
				Add(CGInsn.Build<Add>(Left.MakeArg(1), Right.MakeArg(1), ret.MakeArg(1)));
				Add(CGInsn.Build<Adc>(Left.MakeArg(0), Right.MakeArg(0), ret.MakeArg(0)));
			}
			else if (Left.Type.Size == 8)
			{
				Add(CGInsn.Build<Add>(Left.MakeArg(3), Right.MakeArg(3), ret.MakeArg(3)));
				Add(CGInsn.Build<Adc>(Left.MakeArg(2), Right.MakeArg(2), ret.MakeArg(2)));
				Add(CGInsn.Build<Adc>(Left.MakeArg(1), Right.MakeArg(1), ret.MakeArg(1)));
				Add(CGInsn.Build<Adc>(Left.MakeArg(0), Right.MakeArg(0), ret.MakeArg(0)));
			}
			else throw new NotImplementedException();
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

			if (Left.Type.Size <= 2) Add(CGInsn.Build<Subtract>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
			else if (Left.Type.Size == 4)
			{
				Add(CGInsn.Build<Subtract>(Left.MakeArg(1), Right.MakeArg(1), ret.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(Left.MakeArg(0), Right.MakeArg(0), ret.MakeArg(0)));
			}
			else if (Left.Type.Size == 8)
			{
				Add(CGInsn.Build<Subtract>(Left.MakeArg(3), Right.MakeArg(3), ret.MakeArg(3)));
				Add(CGInsn.Build<Sbb>(Left.MakeArg(2), Right.MakeArg(2), ret.MakeArg(2)));
				Add(CGInsn.Build<Sbb>(Left.MakeArg(1), Right.MakeArg(1), ret.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(Left.MakeArg(0), Right.MakeArg(0), ret.MakeArg(0)));
			}
			else throw new NotImplementedException();
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

			if (Left.Type.IsSigned())
			{
				if (Left.Type.Size == 1)
				{
					Add(CGInsn.Build<SignExtend8>(Left.MakeArg(), Left.MakeArg()));
					Add(CGInsn.Build<SignExtend8>(Right.MakeArg(), Right.MakeArg()));
				}

				if (Left.Type.Size <= 2) Add(CGInsn.Build<SignedMul>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 4) Add(CGInsn.Build<SignedMul32>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 8) Add(CGInsn.Build<SignedMul64>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
			else
			{
				if (Left.Type.Size <= 2) Add(CGInsn.Build<UnsignedMul>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 4) Add(CGInsn.Build<UnsignedMul32>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 8) Add(CGInsn.Build<UnsignedMul64>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
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

			if (Left.Type.IsSigned())
			{
				if (Left.Type.Size == 1)
				{
					Add(CGInsn.Build<SignExtend8>(Left.MakeArg(), Left.MakeArg()));
					Add(CGInsn.Build<SignExtend8>(Right.MakeArg(), Right.MakeArg()));
				}

				if (Left.Type.Size <= 2) Add(CGInsn.Build<SignedDiv>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 4) Add(CGInsn.Build<SignedDiv32>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 8) Add(CGInsn.Build<SignedDiv64>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
			else
			{
				if (Left.Type.Size <= 2) Add(CGInsn.Build<UnsignedDiv>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 4) Add(CGInsn.Build<UnsignedDiv32>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 8) Add(CGInsn.Build<UnsignedDiv64>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
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

			if (Left.Type.IsSigned())
			{
				if (Left.Type.Size == 1)
				{
					Add(CGInsn.Build<SignExtend8>(Left.MakeArg(), Left.MakeArg()));
					Add(CGInsn.Build<SignExtend8>(Right.MakeArg(), Right.MakeArg()));
				}

				if (Left.Type.Size <= 2) Add(CGInsn.Build<SignedModulo>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 4) Add(CGInsn.Build<SignedModulo32>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 8) Add(CGInsn.Build<SignedModulo64>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
			else
			{
				if (Left.Type.Size <= 2) Add(CGInsn.Build<Modulo>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 4) Add(CGInsn.Build<Modulo32>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 8) Add(CGInsn.Build<Modulo64>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
		}
	}

	public class IRInclusiveOr(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Left.Type);

			if (Left.Type.Size == 1)
			{
				Add(CGInsn.Build<Or>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				return;
			}

			for (int i = Left.Type.Size - 2; i >= 0; i -= 2)
			{
				Add(CGInsn.Build<Or>(Left.MakeArg(i / 2), Right.MakeArg(i / 2), ret.MakeArg(i / 2)));
			}
		}
	}

	public class IRExclusiveOr(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Left.Type);

			if (Left.Type.Size == 1)
			{
				Add(CGInsn.Build<Xor>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				return;
			}

			for (int i = Left.Type.Size - 2; i >= 0; i -= 2)
			{
				Add(CGInsn.Build<Xor>(Left.MakeArg(i / 2), Right.MakeArg(i / 2), ret.MakeArg(i / 2)));
			}
		}
	}

	public class IRAnd(IValue left, IValue right) : IRInsn
	{
		public readonly IValue Left = left;
		public readonly IValue Right = right;

		public override void Compile(CodeGen gen)
		{
			var ret = GetReturn(Left.Type);

			if (Left.Type.Size == 1)
			{
				Add(CGInsn.Build<And>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				return;
			}

			for (int i = Left.Type.Size - 2; i >= 0; i -= 2)
			{
				Add(CGInsn.Build<And>(Left.MakeArg(i / 2), Right.MakeArg(i / 2), ret.MakeArg(i / 2)));
			}
		}
	}

	public class IRPostIncrement(IValue val) : IRInsn
	{
		public readonly IValue Value = val;

		public override void Compile(CodeGen gen)
		{
			Return(Value);

			var regs = new SimpleRegisterValue(Alloc(Value.Type.Size), Value.Type);
			if (Value.Type.Size <= 2)
			{
				Add(CGInsn.Build<Add>(new CGRegister(FitRetRegs(Value.Type.Size)[0]), new CGImmediate<ushort>(1), regs.MakeArg()));
			}
			else if (Value.Type.Size == 4)
			{
				Add(CGInsn.Build<Add>(new CGRegister(FitRetRegs(Value.Type.Size)[1]), new CGImmediate<ushort>(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(new CGRegister(FitRetRegs(Value.Type.Size)[0]), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else if (Value.Type.Size == 8)
			{
				Add(CGInsn.Build<Add>(new CGRegister(FitRetRegs(Value.Type.Size)[3]), new CGImmediate<ushort>(1), regs.MakeArg(3)));
				Add(CGInsn.Build<Adc>(new CGRegister(FitRetRegs(Value.Type.Size)[2]), new CGImmediate<ushort>(0), regs.MakeArg(2)));
				Add(CGInsn.Build<Adc>(new CGRegister(FitRetRegs(Value.Type.Size)[1]), new CGImmediate<ushort>(0), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(new CGRegister(FitRetRegs(Value.Type.Size)[0]), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			if (Value is not IPointerValue val) throw new InvalidOperationException();
			MoveRegsToPtr(regs.Registers, val.Pointer);
		}
	}

	public class IRPostDecrement(IValue val) : IRInsn
	{
		public readonly IValue Value = val;

		public override void Compile(CodeGen gen)
		{
			Return(Value);

			var regs = new SimpleRegisterValue(Alloc(Value.Type.Size), Value.Type);
			if (Value.Type.Size <= 2)
			{
				Add(CGInsn.Build<Subtract>(new CGRegister(FitRetRegs(Value.Type.Size)[0]), new CGImmediate<ushort>(1), regs.MakeArg()));
			}
			else if (Value.Type.Size == 4)
			{
				Add(CGInsn.Build<Subtract>(new CGRegister(FitRetRegs(Value.Type.Size)[1]), new CGImmediate<ushort>(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(new CGRegister(FitRetRegs(Value.Type.Size)[0]), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else if (Value.Type.Size == 8)
			{
				Add(CGInsn.Build<Subtract>(new CGRegister(FitRetRegs(Value.Type.Size)[3]), new CGImmediate<ushort>(1), regs.MakeArg(3)));
				Add(CGInsn.Build<Sbb>(new CGRegister(FitRetRegs(Value.Type.Size)[2]), new CGImmediate<ushort>(0), regs.MakeArg(2)));
				Add(CGInsn.Build<Sbb>(new CGRegister(FitRetRegs(Value.Type.Size)[1]), new CGImmediate<ushort>(0), regs.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(new CGRegister(FitRetRegs(Value.Type.Size)[0]), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			if (Value is not IPointerValue val) throw new InvalidOperationException();
			MoveRegsToPtr(regs.Registers, val.Pointer);
		}
	}

	public class IRPreIncrement(IValue val) : IRInsn
	{
		public readonly IValue Value = val;

		public override void Compile(CodeGen gen)
		{
			var regs = Value.ToRegisters(this);
			if (Value.Type.Size <= 2)
			{
				Add(CGInsn.Build<Increment>(regs.MakeArg()));
			}
			else if (Value.Type.Size == 4)
			{
				Add(CGInsn.Build<Add>(regs.MakeArg(1), new CGImmediate<ushort>(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(0), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else if (Value.Type.Size == 8)
			{
				Add(CGInsn.Build<Add>(regs.MakeArg(3), new CGImmediate<ushort>(1), regs.MakeArg(3)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(2), new CGImmediate<ushort>(0), regs.MakeArg(2)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(1), new CGImmediate<ushort>(0), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(0), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			if (Value is not IPointerValue val) throw new InvalidOperationException();
			MoveRegsToPtr(regs.Registers, val.Pointer);
			Return(regs);
		}
	}
	
	public class IRPreDecrement(IValue val) : IRInsn
	{
		public readonly IValue Value = val;

		public override void Compile(CodeGen gen)
		{
			var regs = Value.ToRegisters(this);
			if (Value.Type.Size <= 2)
			{
				Add(CGInsn.Build<Decrement>(regs.MakeArg()));
			}
			else if (Value.Type.Size == 4)
			{
				Add(CGInsn.Build<Subtract>(regs.MakeArg(1), new CGImmediate<ushort>(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(regs.MakeArg(0), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else if (Value.Type.Size == 8)
			{
				Add(CGInsn.Build<Subtract>(regs.MakeArg(3), new CGImmediate<ushort>(1), regs.MakeArg(3)));
				Add(CGInsn.Build<Sbb>(regs.MakeArg(2), new CGImmediate<ushort>(0), regs.MakeArg(2)));
				Add(CGInsn.Build<Sbb>(regs.MakeArg(1), new CGImmediate<ushort>(0), regs.MakeArg(1)));
				Add(CGInsn.Build<Sbb>(regs.MakeArg(0), new CGImmediate<ushort>(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			if (Value is not IPointerValue val) throw new InvalidOperationException();
			MoveRegsToPtr(regs.Registers, val.Pointer);
			Return(regs);
		}
	}
}
