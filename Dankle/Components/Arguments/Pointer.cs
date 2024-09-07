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

		public uint GetAddress() => Ctx.Data[ArgNum] switch
		{
			0b0010 => Utils.Merge(Ctx.Core.GetNext(), Ctx.Core.GetNext()),

			// We love it when C# pretends that uint is long
			0b0011 => (uint)(Utils.Merge(Ctx.Core.GetNext(), Ctx.Core.GetNext()) + (short)Ctx.Core.Registers[Ctx.Core.GetNext<byte>()]),
			0b0100 => (uint)(Utils.Merge(Ctx.Core.Registers[Ctx.Core.GetNext<byte>()], Ctx.Core.Registers[Ctx.Core.GetNext<byte>()]) + (short)Ctx.Core.GetNext()),
			0b0101 => (uint)(Utils.Merge(Ctx.Core.Registers[Ctx.Core.GetNext<byte>()], Ctx.Core.Registers[Ctx.Core.GetNext<byte>()]) + (short)Ctx.Core.Registers[Ctx.Core.GetNext<byte>()]),

			_ => throw new ArgumentException($"Invalid type {Ctx.Data[ArgNum]} for pointer argument"),
		};
	}
}
