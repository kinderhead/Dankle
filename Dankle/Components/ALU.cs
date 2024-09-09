using System;
using System.Numerics;

namespace Dankle.Components
{
    public class ALU(CPUCore core)
    {
        public readonly CPUCore Core = core;

        public T Calculate<T>(T left, Operation op, T right, bool skipFlags = false) where T : INumber<T>
        {
            if (!skipFlags && TypeInfo<T>.Size > 4) throw new InvalidOperationException("Long number types are not supported by ");

            T ret = op switch
            {
                Operation.ADD => left + right,
                Operation.SUB => left - right,
                Operation.MUL => left * right,
                Operation.DIV => left / right,
                Operation.MOD => left % right,
                _ => throw new ArgumentException($"Invalid operation {op}"),
            };

            if (skipFlags) return ret;

            Core.Zero = ret.Equals(0);
            if (!TypeInfo<T>.IsFloatingPoint && op != Operation.MOD)
            {
                if (TypeInfo<T>.IsUnsigned) Core.Overflow = Calculate(ulong.CreateTruncating(left), op, ulong.CreateTruncating(right), true) != ulong.CreateTruncating(ret);
                else Core.Overflow = Calculate(long.CreateTruncating(left), op, long.CreateTruncating(right), true) != long.CreateTruncating(ret);
            }

            return ret;
        }

        public T Shift<T>(T left, ShiftOperation op, T right, bool skipFlags = false) where T : IBinaryInteger<T>, IShiftOperators<T, T, T>
        {
            if (right.Equals(0))
            {
                if (!skipFlags)
                {
                    Core.Zero = left.Equals(0);
                    Core.Overflow = false;
                }
                return left;
            }

            T ret;
            if (op == ShiftOperation.LSH) ret = left << right;
            else if (op == ShiftOperation.RSH) ret = left >> right;
            else throw new ArgumentException($"Invalid operation {op}");

            if (skipFlags) return ret;

            Core.Zero = ret.Equals(0);
            Core.Overflow = T.PopCount(ret) != T.PopCount(left);

			return ret;
        }
    }

    public enum Operation
    {
        ADD,
        SUB,
        MUL,
        DIV,
        MOD
    }

    public enum ShiftOperation
    {
		LSH,
		RSH
	}

    public enum BitwiseOperation
    {
		OR,
		AND
	}
}
