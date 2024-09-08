﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Dankle
{
	public static class Utils
	{
		public static uint Merge(ushort a, ushort b) => (uint)a << 16 | b;
		public static ushort Merge(byte a, byte b) => BitConverter.ToUInt16([a, b]);
	}

	public static class TypeInfo<T>
	{
		public readonly static uint Size;
		public readonly static bool IsUnsigned;
		public readonly static bool IsFloatingPoint;
		public readonly static bool ImplementsIShiftOperators;

		static TypeInfo()
		{
			var dm = new DynamicMethod("SizeOfType", typeof(uint), []);
			ILGenerator il = dm.GetILGenerator();
			il.Emit(OpCodes.Sizeof, typeof(T));
			il.Emit(OpCodes.Ret);

			Size = (uint?)dm.Invoke(null, null) ?? throw new Exception($"Could not find the size of {typeof(T).Name}");
			IsUnsigned = new Type[] { typeof(ushort), typeof(uint), typeof(ulong), typeof(byte) }.Contains(typeof(T));
			IsFloatingPoint = new Type[] { typeof(float), typeof(double) }.Contains(typeof(T));
			ImplementsIShiftOperators = typeof(T).GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IShiftOperators<,,>));
		}
	}
}
