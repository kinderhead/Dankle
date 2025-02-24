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
            IValue part1Arg1;
            IValue part2Arg1;
            IValue part1Arg2;
            IValue part2Arg2;

            if (left is Immediate32 imm1)
            {
                part1Arg1 = new Immediate((ushort)(imm1.Value >> 16), BuiltinType.UnsignedShort);
                part2Arg1 = new Immediate((ushort)(imm1.Value & 0xFFFF), BuiltinType.UnsignedShort);
            }
            else if (left is IRegisterValue reg1)
            {
                part1Arg1 = new SimpleRegisterValue([reg1.Registers[0]], new BuiltinTypeSpecifier(BuiltinType.UnsignedShort));
                part2Arg1 = new SimpleRegisterValue([reg1.Registers[1]], new BuiltinTypeSpecifier(BuiltinType.UnsignedShort));
            }
            else if (left is IPointerValue ptr1)
            {
                part1Arg1 = new SimplePointerValue(ptr1.Pointer.Get(0, 2), new BuiltinTypeSpecifier(BuiltinType.UnsignedShort), insn.Scope);
                part2Arg1 = new SimplePointerValue(ptr1.Pointer.Get(2, 2), new BuiltinTypeSpecifier(BuiltinType.UnsignedShort), insn.Scope);
            }
            else throw new NotImplementedException();

            if (right is Immediate32 imm2)
            {
                part1Arg2 = new Immediate((ushort)(imm2.Value >> 16), BuiltinType.UnsignedShort);
                part2Arg2 = new Immediate((ushort)(imm2.Value & 0xFFFF), BuiltinType.UnsignedShort);
            }
            else if (right is IRegisterValue reg2)
            {
                part1Arg2 = new SimpleRegisterValue([reg2.Registers[0]], new BuiltinTypeSpecifier(BuiltinType.UnsignedShort));
                part2Arg2 = new SimpleRegisterValue([reg2.Registers[1]], new BuiltinTypeSpecifier(BuiltinType.UnsignedShort));
            }
            else if (right is IPointerValue ptr2)
            {
                part1Arg2 = new SimplePointerValue(ptr2.Pointer.Get(0, 2), new BuiltinTypeSpecifier(BuiltinType.UnsignedShort), insn.Scope);
                part2Arg2 = new SimplePointerValue(ptr2.Pointer.Get(2, 2), new BuiltinTypeSpecifier(BuiltinType.UnsignedShort), insn.Scope);
            }
            else throw new NotImplementedException();

            insn.Add(CGInsn.Build<TCompare>(part1Arg1.MakeArg(), part1Arg2.MakeArg()));
            insn.Add(CGInsn.Build<TJump>(new CGLabel<uint>(zero)));
            insn.Add(CGInsn.Build<TCompare>(part2Arg1.MakeArg(), part2Arg2.MakeArg()));
            insn.Add(CGInsn.Build<TJump>(new CGLabel<uint>(zero)));
        }

        public static void SmallOp<TCompareSigned, TCompareUnsigned, TCompareSigned32, TCompareUnsigned32>(IRInsn insn, IValue left, IValue right, SimpleRegisterValue ret) where TCompareSigned : Instruction, new() where TCompareUnsigned : Instruction, new() where TCompareSigned32 : Instruction, new() where TCompareUnsigned32 : Instruction, new()
        {
            if (left.Type.IsSigned())
            {
                if (left.Type.Size == 1)
                {
                    insn.Add(CGInsn.Build<TCompareSigned>(left.ToRegisters(insn).MakeArg(), right.ToRegisters(insn).MakeArg()));
                    insn.Add(CGInsn.Build<GetCompare>(ret.MakeArg()));
                }
                else if (left.Type.Size == 2)
                {
                    insn.Add(CGInsn.Build<TCompareSigned>(left.MakeArg(), right.MakeArg()));
                    insn.Add(CGInsn.Build<GetCompare>(ret.MakeArg()));
                }
                else if (left.Type.Size == 4)
                {
                    
                }
                else throw new NotImplementedException();
            }
        }
    }

    public class IREq(IValue left, IValue right) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            if (Left.Type.Size == 1)
            {
                Add(CGInsn.Build<Compare>(Left.ToRegisters(this).MakeArg(), Right.ToRegisters(this).MakeArg()));
                Add(CGInsn.Build<GetCompare>(new CGRegister(ret.Registers[0])));
            }
            else if (Left.Type.Size == 2)
            {
                Add(CGInsn.Build<Compare>(Left.MakeArg(), Right.MakeArg()));
                Add(CGInsn.Build<GetCompare>(new CGRegister(ret.Registers[0])));
            }
            else if (Left.Type.Size == 4)
            {
                var zero = gen.GetLogicLabel();
                var one = gen.GetLogicLabel();

                LogicUtils.LargeOp32<Compare, JumpNeq>(this, Left, Right, zero);
                Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(1)));
                Add(CGInsn.Build<Jump>(new CGLabel<uint>(one)));
                Add(zero);
                Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(0)));
                Add(one);
            }
            else throw new NotImplementedException();
        }
    }

    public class IRNeq(IValue left, IValue right) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            if (Left.Type.Size == 1)
            {
                Add(CGInsn.Build<Compare>(Left.ToRegisters(this).MakeArg(), Right.ToRegisters(this).MakeArg()));
                Add(CGInsn.Build<GetNotCompare>(new CGRegister(ret.Registers[0])));
            }
            else if (Left.Type.Size == 2)
            {
                Add(CGInsn.Build<Compare>(Left.MakeArg(), Right.MakeArg()));
                Add(CGInsn.Build<GetNotCompare>(new CGRegister(ret.Registers[0])));
            }
            else if (Left.Type.Size == 4)
            {
                var zero = gen.GetLogicLabel();
                var one = gen.GetLogicLabel();

                LogicUtils.LargeOp32<Compare, JumpEq>(this, Left, Right, zero);
                Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(1)));
                Add(CGInsn.Build<Jump>(new CGLabel<uint>(one)));
                Add(zero);
                Add(CGInsn.Build<Load8>(ret.MakeArg(), new CGImmediate<byte>(0)));
                Add(one);
            }
            else throw new NotImplementedException();
        }
    }

    public class IRLt(IValue left, IValue right) : IRInsn
    {
        public readonly IValue Left = left;
        public readonly IValue Right = right;

        public override void Compile(CodeGen gen)
        {
            var ret = GetReturn(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar));

            LogicUtils.SmallOp<LessThan, UnsignedLessThan, LessThan32, LessThan32>(this, Left, Right, ret);
        }
    }
}
