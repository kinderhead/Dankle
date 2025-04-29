using Dankle.Components.CodeGen;
using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public interface IPointer
	{
		public int Size { get; }

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T>;
		public IPointer Get(int offset);
		public IPointer Get(int offset, int size);

		public bool UsingRegister(int reg);
	}

	// public readonly struct Pointer(CGPointer pointer) : IPointer
	// {
	// 	public readonly CGPointer Ptr = pointer;

	// 	public CGPointer Get<T>(IRScope scope) where T : IBinaryInteger<T> => Ptr;
	// }

	public readonly struct StackPointer(int offset, int size) : IPointer
	{
		public readonly int Offset = offset;
		public int Size { get; } = size;

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T>
		{
			if (scope.MaxFuncAllocStackUsed + Offset == 0) return CGPointer<T>.Make(12, 13);
			else return CGPointer<T>.Make(12, 13, (short)(scope.MaxFuncAllocStackUsed + Offset));
		}

		public IPointer Get(int offset) => Get(offset, Size - offset);
		public IPointer Get(int offset, int size)
		{
			if (Size - offset <= 0) throw new InvalidOperationException("StackPointer goes out of bounds");
			return new StackPointer(Offset + offset, size);
		}

		public bool UsingRegister(int reg) => false;
	}

	public readonly struct TempStackPointer(int offset, int size) : IPointer
	{
		public readonly int Offset = offset;
		public int Size { get; } = size;

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T>
		{
			var effectiveOffset = scope.MaxFuncAllocStackUsed + Offset + scope.StackUsed;

			if (effectiveOffset == 0) return CGPointer<T>.Make(12, 13);
			else return CGPointer<T>.Make(12, 13, (short)effectiveOffset);
		}

		public IPointer Get(int offset) => Get(offset, Size - offset);
		public IPointer Get(int offset, int size)
		{
			if (Size - offset <= 0) throw new InvalidOperationException("TempStackPointer goes out of bounds");
			return new TempStackPointer(Offset + offset, size);
		}

		public bool UsingRegister(int reg) => false;
	}

	public readonly struct PreArgumentPointer(int offset, int size) : IPointer
	{
		public readonly int Offset = offset;
		public int Size { get; } = size;

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T>
		{
			var effectiveOffset = Offset;

			if (effectiveOffset == 0) return CGPointer<T>.Make(12, 13);
			else return CGPointer<T>.Make(12, 13, (short)effectiveOffset);
		}

		public IPointer Get(int offset) => Get(offset, Size - offset);
		public IPointer Get(int offset, int size)
		{
			if (Size - offset <= 0) throw new InvalidOperationException("PreArgumentPointer goes out of bounds");
			return new PreArgumentPointer(Offset + offset, size);
		}

		public bool UsingRegister(int reg) => false;
	}

	public readonly struct PostArgumentPointer(int offset, int size) : IPointer
	{
		public readonly int Offset = offset;
		public int Size { get; } = size;

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T>
		{
			var effectiveOffset = scope.EffectiveStackUsed + Offset + 4;

			if (effectiveOffset == 0) return CGPointer<T>.Make(12, 13);
			else return CGPointer<T>.Make(12, 13, (short)effectiveOffset);
		}

		public IPointer Get(int offset) => Get(offset, Size - offset);
		public IPointer Get(int offset, int size)
		{
			if (Size - offset <= 0) throw new InvalidOperationException("PostArgumentPointer goes out of bounds");
			return new PostArgumentPointer(Offset + offset, size);
		}

		public bool UsingRegister(int reg) => false;
	}

	public readonly struct RegisterPointer(int r1, int r2, int offset, int size) : IPointer
	{
		public readonly int Reg1 = r1;
		public readonly int Reg2 = r2;
		public readonly int Offset = offset;
		public int Size { get; } = size;

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T>
		{
			if (Offset == 0) return CGPointer<T>.Make(Reg1, Reg2);
			else return CGPointer<T>.Make(Reg1, Reg2, (short)Offset);
		}

		public IPointer Get(int offset) => Get(offset, Size - offset);
		public IPointer Get(int offset, int size)
		{
			if (Size - offset <= 0) throw new InvalidOperationException("RegisterPointer goes out of bounds");
			return new RegisterPointer(Reg1, Reg2, Offset + offset, Size - offset);
		}

		public bool UsingRegister(int reg) => reg == Reg1 || reg == Reg2;
	}

	public readonly struct LiteralPointer(uint addr, int size) : IPointer
	{
		public readonly uint Address = addr;
		public int Size => size;

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T> => CGPointer<T>.Make(Address);

		public IPointer Get(int offset) => Get(offset, Size - offset);
		public IPointer Get(int offset, int size) => new LiteralPointer(Address + (uint)offset, size);

		public bool UsingRegister(int reg) => false;
	}
	
	public readonly struct LabelPointer(string label, int offset, int size) : IPointer
	{
		public readonly string Address = label;
		public readonly int Offset = offset;
		public int Size => size;

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T> => Offset == 0 ? CGPointer<T>.Make(Address) : CGPointer<T>.Make(Address, (short)Offset);

		public IPointer Get(int offset) => Get(offset, Size - offset);
		public IPointer Get(int offset, int size) => new LabelPointer(Address, Offset + offset, size);

		public bool UsingRegister(int reg) => false;
	}
}
