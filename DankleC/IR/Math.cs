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
				else throw new NotImplementedException();
			}
			else
			{
				if (Left.Type.Size <= 2) Add(CGInsn.Build<UnsignedMul>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 4) Add(CGInsn.Build<UnsignedMul32>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
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
				else throw new NotImplementedException();
			}
			else
			{
				if (Left.Type.Size <= 2) Add(CGInsn.Build<UnsignedDiv>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else if (Left.Type.Size == 4) Add(CGInsn.Build<UnsignedDiv32>(Left.MakeArg(), Right.MakeArg(), ret.MakeArg()));
				else throw new NotImplementedException();
			}
		}
	}

	public class IRPostIncrement(IValue val) : IRInsn
	{
		public readonly IValue Value = val;

		public override void Compile(CodeGen gen)
		{
			Return(Value);

			var regs = Value.ToRegisters(this);
			if (Value.Type.Size <= 2)
			{
				Add(CGInsn.Build<Increment>(regs.MakeArg()));
			}
			else if (Value.Type.Size == 4)
			{
				var inc = new Immediate32(1, ASTObjects.BuiltinType.UnsignedInt).ToRegisters(this);
				Add(CGInsn.Build<Add>(regs.MakeArg(1), inc.MakeArg(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(0), inc.MakeArg(0), regs.MakeArg(0)));
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
				var inc = new Immediate32(1, ASTObjects.BuiltinType.UnsignedInt).ToRegisters(this);
				Add(CGInsn.Build<Add>(regs.MakeArg(1), inc.MakeArg(1), regs.MakeArg(1)));
				Add(CGInsn.Build<Adc>(regs.MakeArg(0), inc.MakeArg(0), regs.MakeArg(0)));
			}
			else throw new NotImplementedException();

			if (Value is not IPointerValue val) throw new InvalidOperationException();
			MoveRegsToPtr(regs.Registers, val.Pointer);
			Return(regs);
        }
    }
}
