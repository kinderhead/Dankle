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
			if (Offset == 0) return CGPointer<T>.Make(12, 13);
			else return CGPointer<T>.Make(12, 13, (short)Offset);
		}

        public IPointer Get(int offset) => Get(offset, Size - offset);
        public IPointer Get(int offset, int size)
        {
        	if (Size - offset <= 0) throw new InvalidOperationException("StackPointer goes out of bounds");
			return new StackPointer(Offset + offset, size);
        }
    }

	public readonly struct TempStackPointer(int offset, int size) : IPointer
	{
		public readonly int Offset = offset;
		public int Size { get; } = size;

		public CGPointer Build<T>(IRScope scope) where T : IBinaryInteger<T>
		{
			var effectiveOffset = Offset + scope.StackUsed;

			if (effectiveOffset == 0) return CGPointer<T>.Make(12, 13);
			else return CGPointer<T>.Make(12, 13, (short)effectiveOffset);
		}

		public IPointer Get(int offset) => Get(offset, Size - offset);
        public IPointer Get(int offset, int size)
        {
        	if (Size - offset <= 0) throw new InvalidOperationException("TempStackPointer goes out of bounds");
			return new TempStackPointer(Offset + offset, Size - offset);
		}
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
	}
}
