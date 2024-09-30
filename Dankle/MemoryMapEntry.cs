using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle
{
	public abstract class MemoryMapEntry : IComparable<MemoryMapEntry>
	{
		/// <summary>
		/// Inclusive
		/// </summary>
		public abstract uint StartAddr { get; }

		/// <summary>
		/// Exclusive
		/// </summary>
		public abstract uint EndAddr { get; }

		public int CompareTo(MemoryMapEntry? other)
		{
			if (other is null) return 1;
			return EndAddr.CompareTo(other.StartAddr);
		}

		public abstract byte[] Read(uint addr, uint size);
		public abstract void Write(uint addr, byte[] data);
	}

	public class RAM(uint end) : MemoryMapEntry
	{
		public override uint StartAddr => 0;

		public override uint EndAddr => end;

		private readonly byte[] Memory = new byte[end];
		private readonly Lock MemoryLock = new();

		public override byte[] Read(uint addr, uint size)
		{
			var data = new byte[size];
			lock (MemoryLock)
			{
				Array.Copy(Memory, addr, data, 0, size);
			}
			return data;
		}

		public override void Write(uint addr, byte[] data)
		{
			lock (MemoryLock)
			{
				data.CopyTo(Memory, addr);
			}
		}
	}
}
