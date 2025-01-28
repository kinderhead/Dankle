﻿using Dankle.Components.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.IR
{
	public interface IPointer
	{
		public CGPointer Get<T>(IRScope scope) where T : IBinaryInteger<T>;
	}

	public readonly struct Pointer(CGPointer pointer) : IPointer
	{
		public readonly CGPointer Ptr = pointer;

		public CGPointer Get<T>(IRScope scope) where T : IBinaryInteger<T> => Ptr;
	}

	public readonly struct StackPointer(int offset, int size) : IPointer
	{
		public readonly int Offset = offset;
		public readonly int Size = size;

		public CGPointer Get<T>(IRScope scope) where T : IBinaryInteger<T>
		{
			if (Offset == 0) return CGPointer<T>.Make(12, 13);
			else return CGPointer<T>.Make(12, 13, (short)Offset);
		}

		public StackPointer GetByte(int offset) => new(Offset + offset, 1);
		public StackPointer GetWord(int offset) => new(Offset + offset, 2);
	}
}
