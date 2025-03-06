using System;
using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using DankleC.ASTObjects;

namespace DankleC.IR
{
    public static class LogicUtils
    {
        public static void LargeOp32<TCompare, TJump>(IRInsn insn, IValue left, IValue right, string zero) where TCompare : Instruction, new() where TJump : Instruction, new()
        {
            insn.Add(CGInsn.Build<TCompare>(left.MakeArg(0), right.MakeArg(0)));
            insn.Add(CGInsn.Build<TJump>(new CGLabel<uint>(zero)));
            insn.Add(CGInsn.Build<TCompare>(left.MakeArg(1), right.MakeArg(1)));
            insn.Add(CGInsn.Build<TJump>(new CGLabel<uint>(zero)));
        }

		public static void LargeOp64<TCompare, TJump>(IRInsn insn, IValue left, IValue right, string zero) where TCompare : Instruction, new() where TJump : Instruction, new()
		{
			insn.Add(CGInsn.Build<TCompare>(left.MakeArg(0), right.MakeArg(0)));
			insn.Add(CGInsn.Build<TJump>(new CGLabel<uint>(zero)));
			insn.Add(CGInsn.Build<TCompare>(left.MakeArg(1), right.MakeArg(1)));
			insn.Add(CGInsn.Build<TJump>(new CGLabel<uint>(zero)));
			insn.Add(CGInsn.Build<TCompare>(left.MakeArg(2), right.MakeArg(2)));
			insn.Add(CGInsn.Build<TJump>(new CGLabel<uint>(zero)));
			insn.Add(CGInsn.Build<TCompare>(left.MakeArg(3), right.MakeArg(3)));
			insn.Add(CGInsn.Build<TJump>(new CGLabel<uint>(zero)));
		}

		public static void SmallOp<TCompareSigned, TCompareUnsigned, TCompareSigned32, TCompareUnsigned32, TCompareSigned64, TCompareUnsigned64>(IRInsn insn, IValue left, IValue right) where TCompareSigned : Instruction, new() where TCompareUnsigned : Instruction, new() where TCompareSigned32 : Instruction, new() where TCompareUnsigned32 : Instruction, new() where TCompareSigned64 : Instruction, new() where TCompareUnsigned64 : Instruction, new()
        {
            if (left.Type.IsSigned())
            {
                if (left.Type.Size == 1)
                {
                    var leftArg = left;
                    var rightArg = right;

                    if (leftArg is IPointerValue)
                    {
                        leftArg = left.ToRegisters(insn);
                        insn.Add(CGInsn.Build<SignExtend8>(leftArg.MakeArg(), leftArg.MakeArg()));
                    }

                    if (rightArg is IPointerValue)
                    {
                        rightArg = right.ToRegisters(insn);
                        insn.Add(CGInsn.Build<SignExtend8>(rightArg.MakeArg(), rightArg.MakeArg()));
                    }

                    insn.Add(CGInsn.Build<TCompareSigned>(leftArg.MakeArg(), rightArg.MakeArg()));
                }
                else if (left.Type.Size == 2)
                {
                    insn.Add(CGInsn.Build<TCompareSigned>(left.MakeArg(), right.MakeArg()));
                }
                else if (left.Type.Size == 4)
                {
                    insn.Add(CGInsn.Build<TCompareSigned32>(left.MakeArg(), right.MakeArg()));
                }
                else if (left.Type.Size == 8)
                {
                    insn.Add(CGInsn.Build<TCompareSigned64>(left.MakeArg(), right.MakeArg()));
                }
                else throw new NotImplementedException();
            }
            else
            {
                if (left.Type.Size == 1)
                {
                    insn.Add(CGInsn.Build<TCompareUnsigned>(left.ToRegisters(insn).MakeArg(), right.ToRegisters(insn).MakeArg()));
                }
                else if (left.Type.Size == 2)
                {
                    insn.Add(CGInsn.Build<TCompareUnsigned>(left.MakeArg(), right.MakeArg()));
                }
                else if (left.Type.Size == 4)
                {
                    insn.Add(CGInsn.Build<TCompareUnsigned32>(left.MakeArg(), right.MakeArg()));
                }
                else if (left.Type.Size == 8)
                {
                    insn.Add(CGInsn.Build<TCompareUnsigned64>(left.MakeArg(), right.MakeArg()));
                }
                else throw new NotImplementedException();
            }
        }
    }

    public class IREq(IValue left, IValue right, bool ret) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;
        public readonly bool ShouldReturn = ret;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            if (Left.Type.Size == 1)
            {
                Add(CGInsn.Build<Compare>(Left.ToRegisters(this).MakeArg(), Right.ToRegisters(this).MakeArg()));
                if (ShouldReturn) Add(CGInsn.Build<GetCompare>(new CGRegister(ret.Registers[0])));
            }
            else if (Left.Type.Size == 2)
            {
                Add(CGInsn.Build<Compare>(Left.MakeArg(), Right.MakeArg()));
                if (ShouldReturn) Add(CGInsn.Build<GetCompare>(new CGRegister(ret.Registers[0])));
            }
            else if (Left.Type.Size == 4)
            {
                var zero = gen.GetLogicLabel();

                if (ShouldReturn)
                {
                    var one = gen.GetLogicLabel();

                    LogicUtils.LargeOp32<Compare, JumpNeq>(this, Left, Right, zero);
                    Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(1)));
                    Add(CGInsn.Build<Jump>(new CGLabel<uint>(one)));
                    Add(zero);
                    Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(0)));
                    Add(one);
                }
                else
                {
                    LogicUtils.LargeOp32<Compare, JumpNeq>(this, Left, Right, zero);
                    Add(zero);
                }
            }
			else if (Left.Type.Size == 8)
			{
				var zero = gen.GetLogicLabel();

				if (ShouldReturn)
				{
					var one = gen.GetLogicLabel();

					LogicUtils.LargeOp64<Compare, JumpNeq>(this, Left, Right, zero);
					Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(1)));
					Add(CGInsn.Build<Jump>(new CGLabel<uint>(one)));
					Add(zero);
					Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(0)));
					Add(one);
				}
				else
				{
					LogicUtils.LargeOp64<Compare, JumpNeq>(this, Left, Right, zero);
					Add(zero);
				}
			}
			else throw new InvalidOperationException();
        }
    }

    public class IRNeq(IValue left, IValue right, bool ret) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;
        public readonly bool ShouldReturn = ret;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            if (Left.Type.Size == 1)
            {
                Add(CGInsn.Build<Compare>(Left.ToRegisters(this).MakeArg(), Right.ToRegisters(this).MakeArg()));
                if (ShouldReturn) Add(CGInsn.Build<GetNotCompare>(new CGRegister(ret.Registers[0])));
                else Add(CGInsn.Build<FlipCompare>());
            }
            else if (Left.Type.Size == 2)
            {
                Add(CGInsn.Build<Compare>(Left.MakeArg(), Right.MakeArg()));
                if (ShouldReturn) Add(CGInsn.Build<GetNotCompare>(new CGRegister(ret.Registers[0])));
                else Add(CGInsn.Build<FlipCompare>());
            }
            else if (Left.Type.Size == 4)
            {
                var zero = gen.GetLogicLabel();

                if (ShouldReturn)
                {
                    var one = gen.GetLogicLabel();

                    LogicUtils.LargeOp32<Compare, JumpNeq>(this, Left, Right, zero);
                    Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(0)));
                    Add(CGInsn.Build<Jump>(new CGLabel<uint>(one)));
                    Add(zero);
                    Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(1)));
                    Add(one);
                }
                else
                {
                    LogicUtils.LargeOp32<Compare, JumpNeq>(this, Left, Right, zero);
                    Add(zero);
                    Add(CGInsn.Build<FlipCompare>());
                }
            }
			else if (Left.Type.Size == 8)
			{
				var zero = gen.GetLogicLabel();

				if (ShouldReturn)
				{
					var one = gen.GetLogicLabel();

					LogicUtils.LargeOp64<Compare, JumpNeq>(this, Left, Right, zero);
					Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(0)));
					Add(CGInsn.Build<Jump>(new CGLabel<uint>(one)));
					Add(zero);
					Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(1)));
					Add(one);
				}
				else
				{
					LogicUtils.LargeOp64<Compare, JumpNeq>(this, Left, Right, zero);
					Add(zero);
					Add(CGInsn.Build<FlipCompare>());
				}
			}
			else throw new InvalidOperationException();
        }
    }

    public class IRLt(IValue left, IValue right, bool ret) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;
        public readonly bool ShouldReturn = ret;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            LogicUtils.SmallOp<LessThan, UnsignedLessThan, LessThan32, UnsignedLessThan32, LessThan64, UnsignedLessThan64>(this, Left, Right);
            if (ShouldReturn) Add(CGInsn.Build<GetCompare>(ret.MakeArg()));
        }
    }

    public class IRLte(IValue left, IValue right, bool ret) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;
        public readonly bool ShouldReturn = ret;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            LogicUtils.SmallOp<LessThanOrEq, UnsignedLessThanOrEq, LessThanOrEq32, UnsignedLessThanOrEq32, LessThanOrEq64, UnsignedLessThanOrEq64>(this, Left, Right);
            if (ShouldReturn) Add(CGInsn.Build<GetCompare>(ret.MakeArg()));
        }
    }

    public class IRGt(IValue left, IValue right, bool ret) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;
        public readonly bool ShouldReturn = ret;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            LogicUtils.SmallOp<GreaterThan, UnsignedGreaterThan, GreaterThan32, UnsignedGreaterThan32, GreaterThan64, UnsignedGreaterThan64>(this, Left, Right);
            if (ShouldReturn) Add(CGInsn.Build<GetCompare>(ret.MakeArg()));
        }
    }

    public class IRGte(IValue left, IValue right, bool ret) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;
        public readonly bool ShouldReturn = ret;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            LogicUtils.SmallOp<GreaterThanOrEq, UnsignedGreaterThanOrEq, GreaterThanOrEq32, UnsignedGreaterThanOrEq32, GreaterThanOrEq64, UnsignedGreaterThanOrEq64>(this, Left, Right);
            if (ShouldReturn) Add(CGInsn.Build<GetCompare>(ret.MakeArg()));
        }
    }

    public class IRJumpEq(ILabel label) : IRInsn
    {
        public readonly ILabel Label = label;

        public override void Compile(CodeGen gen)
        {
            Add(CGInsn.Build<JumpEq>(new CGLabel<uint>(Label.Resolve(gen))));
        }
    }
    
    public class IRJump(ILabel label) : IRInsn
    {
        public readonly ILabel Label = label;

        public override void Compile(CodeGen gen)
        {
            Add(CGInsn.Build<Jump>(new CGLabel<uint>(Label.Resolve(gen))));
        }
    }
    
    public class IRJumpNeq(ILabel label) : IRInsn
    {
        public readonly ILabel Label = label;

        public override void Compile(CodeGen gen)
        {
            Add(CGInsn.Build<JumpNeq>(new CGLabel<uint>(Label.Resolve(gen))));
        }
    }
}
