using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle
{
	public static class Utils
	{
		public static uint Merge(ushort a, ushort b) => (uint)a << 16 | b;
		public static ushort Merge(byte a, byte b) => BitConverter.ToUInt16([a, b]);
	}
}
