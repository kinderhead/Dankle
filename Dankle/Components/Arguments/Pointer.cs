using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Pointer<T> : Argument<T> where T : IBinaryInteger<T>
	{
		public Pointer(Context ctx, int type) : base(ctx, type)
		{
		}

		public Pointer()
		{
		}

		public override IArgument Create(Context ctx, int argnum) => new Pointer<T>(ctx, argnum);

		public override T Read() => Ctx.Core.Computer.ReadMem<T>(GetAddress());
		public override void Write(T value) => Ctx.Core.Computer.WriteMem(GetAddress(), value);

		public uint GetAddress()
		{
			switch (Ctx.Data[ArgNum])
			{
				case 0b0010:
					return Utils.Merge(Ctx.Core.GetNext(), Ctx.Core.GetNext());

				case 0b0011:
					return Utils.Merge(Ctx.Core.GetNext(), Ctx.Core.GetNext()) + Ctx.Core.Registers[Ctx.Core.GetNext<byte>()];
				case 0b0100:
					var registers = Ctx.Core.GetNext<byte>();

					// We love it when C# pretends that uint is long
					return (uint)(Utils.Merge(Ctx.Core.Registers[registers >> 4], Ctx.Core.Registers[registers & 0xF]) + Ctx.Core.GetNext<short>());
				case 0b0101:
					// C# scope moment
					var registers2 = Ctx.Core.GetNext<byte>();

					return Utils.Merge(Ctx.Core.Registers[registers2 >> 4], Ctx.Core.Registers[registers2 & 0xF]) + Ctx.Core.Registers[Ctx.Core.GetNext<byte>()];
				case 0b0110:
					var registers3 = Ctx.Core.GetNext<byte>();

					return Utils.Merge(Ctx.Core.Registers[registers3 >> 4], Ctx.Core.Registers[registers3 & 0xF]);
				case 0b0111:
					return Utils.Merge(Ctx.Core.GetNext(), Ctx.Core.GetNext()) - Ctx.Core.Registers[Ctx.Core.GetNext<byte>()];
				case 0b1000:
					var registers4 = Ctx.Core.GetNext<byte>();

					return Utils.Merge(Ctx.Core.Registers[registers4 >> 4], Ctx.Core.Registers[registers4 & 0xF]) - Ctx.Core.Registers[Ctx.Core.GetNext<byte>()];
				case 0b1001:
					return (uint)(Utils.Merge(0, Ctx.Core.Registers[Ctx.Core.GetNext<byte>()]) + Ctx.Core.GetNext<short>());
				case 0b1010:
					var registers5 = Ctx.Core.GetNext<byte>();

					return Utils.Merge(0, Ctx.Core.Registers[registers5 >> 4]) + Ctx.Core.Registers[Ctx.Core.Registers[registers5 & 0xF]];
				case 0b1011:
					return Ctx.Core.Registers[Ctx.Core.GetNext<byte>()];
				case 0b1100:
					var registers6 = Ctx.Core.GetNext<byte>();

					return Utils.Merge(0, Ctx.Core.Registers[registers6 >> 4]) - Ctx.Core.Registers[Ctx.Core.Registers[registers6 & 0xF]];
				default:
					throw new ArgumentException($"Invalid type {Ctx.Data[ArgNum]} for pointer argument");
			}
		}
	}
}
