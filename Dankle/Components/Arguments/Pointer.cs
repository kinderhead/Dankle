using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Pointer : Argument<byte>
	{
		public override byte Read(CPUCore core, byte type, Func<ushort> supply, ushort[] registers) => core.ReadMem(Utils.Merge(supply(), supply()));
		public override void Write(byte value, CPUCore core, byte type, Func<ushort> supply, ushort[] registers) => core.WriteMem(Utils.Merge(supply(), supply()), value);
	}

	public class Pointer16 : Argument<ushort>
	{
		public override ushort Read(CPUCore core, byte type, Func<ushort> supply, ushort[] registers) => core.ReadMem16(Utils.Merge(supply(), supply()));
		public override void Write(ushort value, CPUCore core, byte type, Func<ushort> supply, ushort[] registers) => core.WriteMem(Utils.Merge(supply(), supply()), value);
	}
}
