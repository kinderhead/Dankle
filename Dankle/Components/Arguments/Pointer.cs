using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Pointer<T>(Context ctx, int type) : Argument<T>(ctx, type) where T : IBinaryInteger<T>
	{
		public override T Read() => Ctx.Core.Computer.ReadMem<T>(GetAddress());
		public override void Write(T value) => Ctx.Core.Computer.WriteMem(GetAddress(), value);

		public uint GetAddress()
		{
			switch (Ctx.Data[ArgNum])
			{
				case 0b0010:
					return Utils.Merge(Ctx.Core.GetNext(), Ctx.Core.GetNext());

				// We love it when C# pretends that uint is long
				case 0b0011:
					return (uint)(Utils.Merge(Ctx.Core.GetNext(), Ctx.Core.GetNext()) + (short)Ctx.Core.Registers[Ctx.Core.GetNext<byte>()]);
				case 0b0100:
					var registers = Ctx.Core.GetNext<byte>();

					return (uint)(Utils.Merge(Ctx.Core.Registers[registers >> 4], Ctx.Core.Registers[registers & 0xFF]) + (short)Ctx.Core.GetNext());
				case 0b0101:
					// C# scope moment
					var registers2 = Ctx.Core.GetNext<byte>();

					return (uint)(Utils.Merge(Ctx.Core.Registers[registers2 >> 4], Ctx.Core.Registers[registers2 & 0xFF]) + (short)Ctx.Core.Registers[Ctx.Core.GetNext<byte>()]);
				default:
					throw new ArgumentException($"Invalid type {Ctx.Data[ArgNum]} for pointer argument");
			}
		}
	}
}
