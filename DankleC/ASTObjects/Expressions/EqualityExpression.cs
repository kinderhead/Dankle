using System;
using DankleC.IR;

namespace DankleC.ASTObjects.Expressions
{
    public enum EqualityOperation
    {
        Equals,
        NotEquals,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    public class EqualityExpression(IExpression left, EqualityOperation op, IExpression right) : UnresolvedExpression
    {
        public readonly IExpression Left = left;
		public readonly EqualityOperation Op = op;
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
			if (!left.Type.IsNumber() || !right.Type.IsNumber() || left.Type is PointerTypeSpecifier || right.Type is PointerTypeSpecifier) throw new InvalidOperationException($"Cannot perform arithmetic between {left.Type} and {right.Type}");

            var type = TypeSpecifier.GetOperationType(left.Type, right.Type);

            if (left is ConstantExpression l && right is ConstantExpression r)
            {
                dynamic res = Op switch
                {
                    EqualityOperation.Equals => (dynamic)l.Value == (dynamic)r.Value,
                    EqualityOperation.NotEquals => (dynamic)l.Value == (dynamic)r.Value,
                    EqualityOperation.LessThan => (dynamic)l.Value < (dynamic)r.Value,
                    EqualityOperation.LessThanOrEqual => (dynamic)l.Value <= (dynamic)r.Value,
                    EqualityOperation.GreaterThan => (dynamic)l.Value > (dynamic)r.Value,
                    EqualityOperation.GreaterThanOrEqual => (dynamic)l.Value >= (dynamic)r.Value,
					_ => throw new NotImplementedException(),
                };

                return new ConstantExpression(type, res);
            }

            if (type.Size == 1 && type.IsSigned() && Op != EqualityOperation.Equals && Op != EqualityOperation.NotEquals) type = new BuiltinTypeSpecifier(BuiltinType.SignedShort);

            left = left.Cast(type);
            right = right.Cast(type);

            return new ResolvedEqualityExpression(left, Op, right, type);
        }
    }

    public class ResolvedEqualityExpression(ResolvedExpression left, EqualityOperation op, ResolvedExpression right, TypeSpecifier type) : ResolvedExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar))
    {
        public readonly ResolvedExpression Left = left;
		public readonly EqualityOperation Op = op;
		public readonly ResolvedExpression Right = right;
        public readonly TypeSpecifier SourceType = type;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedEqualityExpression(Left, Op, Right, SourceType);

        public override void PrepScope(IRScope scope)
        {
            Left.PrepScope(scope);
			Right.PrepScope(scope);
        }

        public void Compute(int[] leftregs, int[] rightregs, int output, IRBuilder builder)
        {
            if (SourceType.Size <= 2)
            {
                switch (Op)
                {
                    case EqualityOperation.Equals:
                        builder.Add(new CmpRegs(leftregs[0], rightregs[0]));
                        builder.Add(new GetC(output));
                        break;
                    case EqualityOperation.NotEquals:
                        builder.Add(new CmpRegs(leftregs[0], rightregs[0]));
                        builder.Add(new GetNC(output));
                        break;
                    case EqualityOperation.LessThan:
                        if (SourceType.IsSigned()) builder.Add(new LTRegs(leftregs[0], rightregs[0]));
                        else builder.Add(new ULTRegs(leftregs[0], rightregs[0]));
                        builder.Add(new GetC(output));
                        break;
                    case EqualityOperation.LessThanOrEqual:
                        if (SourceType.IsSigned()) builder.Add(new LTERegs(leftregs[0], rightregs[0]));
                        else builder.Add(new ULTERegs(leftregs[0], rightregs[0]));
                        builder.Add(new GetC(output));
                        break;
                    case EqualityOperation.GreaterThan:
                        if (SourceType.IsSigned()) builder.Add(new GTRegs(leftregs[0], rightregs[0]));
                        else builder.Add(new UGTRegs(leftregs[0], rightregs[0]));
                        builder.Add(new GetC(output));
                        break;
                    case EqualityOperation.GreaterThanOrEqual:
                        if (SourceType.IsSigned()) builder.Add(new GTERegs(leftregs[0], rightregs[0]));
                        else builder.Add(new UGTERegs(leftregs[0], rightregs[0]));
                        builder.Add(new GetC(output));
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            else if (SourceType.Size == 4)
            {
                var zeroLabel = builder.GetLogicLabel();
                var oneLabel = builder.GetLogicLabel();
                switch (Op)
                {
                    case EqualityOperation.Equals:
                        builder.Add(new CmpRegs(leftregs[0], rightregs[0]));
                        builder.Add(new JumpIfNotTrue(zeroLabel.Name));
                        builder.Add(new CmpRegs(leftregs[1], rightregs[1]));
                        builder.Add(new JumpIfNotTrue(zeroLabel.Name));
                        builder.Add(new LoadImmToReg(output, 1));
                        builder.Add(new JumpTo(oneLabel.Name));
                        builder.Add(zeroLabel);
                        builder.Add(new LoadImmToReg(output, 0));
                        builder.Add(oneLabel);
                        break;
                    case EqualityOperation.NotEquals:
                        builder.Add(new CmpRegs(leftregs[0], rightregs[0]));
                        builder.Add(new JumpIfTrue(zeroLabel.Name));
                        builder.Add(new CmpRegs(leftregs[1], rightregs[1]));
                        builder.Add(new JumpIfTrue(zeroLabel.Name));
                        builder.Add(new LoadImmToReg(output, 1));
                        builder.Add(new JumpTo(oneLabel.Name));
                        builder.Add(zeroLabel);
                        builder.Add(new LoadImmToReg(output, 0));
                        builder.Add(oneLabel);
                        break;
                    case EqualityOperation.LessThan:
                    case EqualityOperation.LessThanOrEqual:
                    case EqualityOperation.GreaterThan:
                    case EqualityOperation.GreaterThanOrEqual:
                        if (SourceType.IsSigned())
                        {
                            builder.Add(Op switch
                            {
                                EqualityOperation.LessThan => new LT32(leftregs[0], leftregs[1], rightregs[0], rightregs[1]),
                                EqualityOperation.LessThanOrEqual => new LTE32(leftregs[0], leftregs[1], rightregs[0], rightregs[1]),
                                EqualityOperation.GreaterThan => new GT32(leftregs[0], leftregs[1], rightregs[0], rightregs[1]),
                                EqualityOperation.GreaterThanOrEqual => new GTE32(leftregs[0], leftregs[1], rightregs[0], rightregs[1]),
                                _ => throw new InvalidOperationException()
                            });
                            builder.Add(new GetC(output));
                        }
                        else
                        {
                            IRInsn cmp1;
                            IRInsn cmp2;

                            if (Op == EqualityOperation.LessThan)
                            {
                                cmp1 = new ULTRegs(leftregs[0], rightregs[0]);
                                cmp2 = new ULTRegs(leftregs[1], rightregs[1]);
                            }
                            else if (Op == EqualityOperation.LessThanOrEqual)
                            {
                                cmp1 = new ULTERegs(leftregs[0], rightregs[0]);
                                cmp2 = new ULTERegs(leftregs[1], rightregs[1]);
                            }
                            else if (Op == EqualityOperation.GreaterThan)
                            {
                                cmp1 = new UGTRegs(leftregs[0], rightregs[0]);
                                cmp2 = new UGTRegs(leftregs[1], rightregs[1]);
                            }
                            else if (Op == EqualityOperation.GreaterThanOrEqual)
                            {
                                cmp1 = new UGTERegs(leftregs[0], rightregs[0]);
                                cmp2 = new UGTERegs(leftregs[1], rightregs[1]);
                            }
                            else throw new InvalidOperationException();

                            builder.Add(cmp1);
                            builder.Add(new JumpIfNotTrue(zeroLabel.Name));
                            builder.Add(cmp2);
                            builder.Add(new JumpIfNotTrue(zeroLabel.Name));
                            builder.Add(new LoadImmToReg(output, 1));
                            builder.Add(new JumpTo(oneLabel.Name));
                            builder.Add(zeroLabel);
                            builder.Add(new LoadImmToReg(output, 0));
                            builder.Add(oneLabel);
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            else throw new NotImplementedException();
        }

        public override void WriteToPointer(IPointer pointer, IRBuilder builder, int[] usedRegs)
        {
            using var tmp1 = builder.CurrentScope.AllocTempRegs(SourceType.Size, usedRegs);
            var leftRegs = Left.GetOrWriteToRegisters(tmp1.Registers, builder);
            using var tmp2 = builder.CurrentScope.AllocTempRegs(SourceType.Size, [.. leftRegs, .. usedRegs]);
            var rightRegs = Right.GetOrWriteToRegisters(tmp2.Registers, builder);
            Compute(leftRegs, rightRegs, tmp1.Registers[0], builder);
            builder.Add(new LoadRegToPtr8(pointer, tmp1.Registers[0]));
        }

        public override void WriteToRegisters(int[] regs, IRBuilder builder)
        {
            if (regs.Length != 1) throw new InvalidOperationException();

            if (SourceType.Size <= 2)
            {
                var leftRegs = Left.GetOrWriteToRegisters(regs, builder);
                using var tmp = builder.CurrentScope.AllocTempRegs(SourceType.Size, regs);
                var rightRegs = Right.GetOrWriteToRegisters(tmp.Registers, builder);
                Compute(leftRegs, rightRegs, regs[0], builder);
            }
            else
            {
                using var tmp1 = builder.CurrentScope.AllocTempRegs(SourceType.Size - 2, regs);
                var leftRegs = Left.GetOrWriteToRegisters([.. regs, .. tmp1.Registers], builder);
                using var tmp2 = builder.CurrentScope.AllocTempRegs(SourceType.Size, [.. leftRegs, .. regs]);
                var rightRegs = Right.GetOrWriteToRegisters(tmp2.Registers, builder);
                Compute(leftRegs, rightRegs, regs[0], builder);
            }
        }
    }
}
