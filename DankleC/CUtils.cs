using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DankleC
{
	public static class CUtils
	{
		public static string NumberTypeToString<T>() where T : INumber<T> => Type.GetTypeCode(typeof(T)) switch
		{
			TypeCode.SByte => "signed char",
			TypeCode.Byte => "char",
			TypeCode.Int16 => "short",
			TypeCode.UInt16 => "unsigned short",
			TypeCode.Int32 => "int",
			TypeCode.UInt32 => "unsigned int",
			_ => throw new InvalidOperationException()
		};
	}
}
