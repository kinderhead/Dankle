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

            Core.Zero = ret == T.AdditiveIdentity;
            if (!TypeInfo<T>.IsFloatingPoint && op != Operation.MOD)
            {
                if (TypeInfo<T>.IsUnsigned) Core.Overflow = Calculate(ulong.CreateTruncating(left), op, ulong.CreateTruncating(right), true) != ulong.CreateTruncating(ret);
                else Core.Overflow = Calculate(long.CreateTruncating(left), op, long.CreateTruncating(right), true) != long.CreateTruncating(ret);
            }

            if (!TypeInfo<T>.IsFloatingPoint && op == Operation.ADD)
            {
                if (TypeInfo<T>.IsUnsigned) Core.Carry = ret < left;
                else throw new NotImplementedException();
            }

            if (!TypeInfo<T>.IsFloatingPoint && op == Operation.SUB)
            {
                if (TypeInfo<T>.IsUnsigned) Core.Carry = left < right;
                else throw new NotImplementedException();
            }

            return ret;
        }

        public T Shift<T>(T left, ShiftOperation op, int right, bool skipFlags = false) where T : IBinaryInteger<T>, IShiftOperators<T, int, T>
        {
            if (right == 0)
            {
                if (!skipFlags)
                {
                    Core.Zero = left == T.AdditiveIdentity;
					Core.Overflow = false;
                }
                return left;
            }

            T ret;
            if (op == ShiftOperation.LSH) ret = left << right;
            else if (op == ShiftOperation.RSH) ret = left >> right;
            else throw new ArgumentException($"Invalid operation {op}");

            if (skipFlags) return ret;

            Core.Zero = ret == T.AdditiveIdentity;
			Core.Overflow = T.PopCount(ret) != T.PopCount(left);

			return ret;
        }

        public T Bitwise<T>(T left, BitwiseOperation op, T right) where T : IBinaryInteger<T>, IBitwiseOperators<T, T, T>
        {
            var ret = op switch
            {
                BitwiseOperation.AND => left & right,
                BitwiseOperation.OR => left | right,
                BitwiseOperation.XOR => left ^ right,
                _ => throw new ArgumentException($"Invalid operation {op}")
            };

			Core.Zero = ret == T.AdditiveIdentity;
            return ret;
		}

        public void CompareAndSetFlag<T>(T left, Comparison op, T right) where T : IComparisonOperators<T, T, bool>
        {
			Core.Compare = op switch
			{
				Comparison.EQ => left == right,
				Comparison.LT => left < right,
				Comparison.GT => left > right,
				Comparison.LTE => left <= right,
				Comparison.GTE => left >= right,
				_ => throw new ArgumentException($"Invalid operation {op}"),
			};
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
		AND,
        XOR
	}

    public enum Comparison
    {
        EQ,
        LT,
        GT,
        LTE,
        GTE
    }
}
