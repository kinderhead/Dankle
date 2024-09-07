using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Any16(Context ctx, int type) : Argument<ushort>(ctx, type)
	{
		public override ushort Read() => GetArg(Ctx.Data[ArgNum]).Read();
		public override void Write(ushort value) => GetArg(Ctx.Data[ArgNum]).Write(value);

		public Argument<ushort> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate<ushort>(Ctx, ArgNum),
			0b0001 => new Register(Ctx, ArgNum),
			0b0010 or 0b0011 or 0b0100 or 0b0101 => new Pointer<ushort>(Ctx, ArgNum),
			_ => throw new ArgumentException($"Invalid type {type} for 16 bit any argument"),
		};
	}

	public class Any16Num(Context ctx, int type) : Argument<ushort>(ctx, type)
	{
		public override ushort Read() => GetArg(Ctx.Data[ArgNum]).Read();
		public override void Write(ushort value) => GetArg(Ctx.Data[ArgNum]).Write(value);

		public Argument<ushort> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate<ushort>(Ctx, ArgNum),
			0b0010 or 0b0011 or 0b0100 or 0b0101 => new Pointer<ushort>(Ctx, ArgNum),
			_ => throw new ArgumentException($"Invalid type {type} for 16 bit any number"),
		};
	}

	public class Any8Num(Context ctx, int type) : Argument<byte>(ctx, type)
	{
		public override byte Read() => GetArg(Ctx.Data[ArgNum]).Read();
		public override void Write(byte value) => GetArg(Ctx.Data[ArgNum]).Write(value);

		public Argument<byte> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate<byte>(Ctx, ArgNum),
			0b0010 or 0b0011 or 0b0100 or 0b0101 => new Pointer<byte>(Ctx, ArgNum),
			_ => throw new ArgumentException($"Invalid type {type} for 16 bit any number"),
		};
	}
}
