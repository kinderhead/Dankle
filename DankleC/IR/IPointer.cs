using Dankle.Components.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public interface IPointer
	{
		public CGPointer Get(IRScope scope);
	}

	public readonly struct Pointer(CGPointer pointer) : IPointer
	{
		public readonly CGPointer Ptr = pointer;

		public CGPointer Get(IRScope scope) => Ptr;
	}

	public readonly struct StackPointer(int offset, int size) : IPointer
	{
		public readonly int Offset = offset;
		public readonly int Size = size;

		public CGPointer Get(IRScope scope)
		{
			throw new NotImplementedException();
		}

		public StackPointer GetByte(int offset) => new(Offset + offset, 1);
		public StackPointer GetWord(int offset) => new(Offset + offset, 2);
	}
}
