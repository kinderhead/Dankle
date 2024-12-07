using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Any16 : Argument<ushort>
	{
		public Any16()
		{
		}

		public Any16(Context ctx, int type) : base(ctx, type)
		{
		}

		public override Type[] AssignableFrom => [typeof(Immediate<ushort>), typeof(Register), typeof(Pointer<ushort>)];

		public override IArgument Create(Context ctx, int argnum) => new Any16(ctx, argnum);

		public override string Dissassemble() => GetArg(Ctx.Data[ArgNum]).Dissassemble();
		public override ushort Read() => GetArg(Ctx.Data[ArgNum]).Read();
		public override void Write(ushort value) => GetArg(Ctx.Data[ArgNum]).Write(value);

		private Argument<ushort> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate<ushort>(Ctx, ArgNum),
			0b0001 => new Register(Ctx, Ctx.Core.GetNext<byte>(), true),
			0b0010 or 0b0011 or 0b0100 or 0b0101 or 0b0110 or 0b0111 or 0b1000 or 0b1001 or 0b1010 or 0b1011 or 0b1100 => new Pointer<ushort>(Ctx, ArgNum),
			_ => throw new ArgumentException($"Invalid type {type} for 16 bit any argument"),
		};
	}

	public class Any16Num : Argument<ushort>
	{
		public Any16Num(Context ctx, int type) : base(ctx, type)
		{
		}

		public Any16Num()
		{
		}

		public override Type[] AssignableFrom => [typeof(Immediate<ushort>), typeof(Pointer<ushort>)];

		public override IArgument Create(Context ctx, int argnum) => new Any16Num(ctx, argnum);

		public override string Dissassemble() => GetArg(Ctx.Data[ArgNum]).Dissassemble();
		public override ushort Read() => GetArg(Ctx.Data[ArgNum]).Read();
		public override void Write(ushort value) => GetArg(Ctx.Data[ArgNum]).Write(value);

		public Argument<ushort> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate<ushort>(Ctx, ArgNum),
			0b0010 or 0b0011 or 0b0100 or 0b0101 or 0b0110 or 0b0111 or 0b1000 or 0b1001 or 0b1010 or 0b1011 or 0b1100 => new Pointer<ushort>(Ctx, ArgNum),
			_ => throw new ArgumentException($"Invalid type {type} for 16 bit number"),
		};
	}

	public class Any8Num : Argument<byte>
	{
		public Any8Num(Context ctx, int type) : base(ctx, type)
		{
		}

		public Any8Num()
		{
		}

		public override Type[] AssignableFrom => [typeof(Immediate<byte>), typeof(Pointer<byte>)];

		public override IArgument Create(Context ctx, int argnum) => new Any8Num(ctx, argnum);

		public override string Dissassemble() => GetArg(Ctx.Data[ArgNum]).Dissassemble();
		public override byte Read() => GetArg(Ctx.Data[ArgNum]).Read();
		public override void Write(byte value) => GetArg(Ctx.Data[ArgNum]).Write(value);

		public Argument<byte> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate<byte>(Ctx, ArgNum),
			0b0010 or 0b0011 or 0b0100 or 0b0101 or 0b0110 or 0b0111 or 0b1000 or 0b1001 or 0b1010 or 0b1011 or 0b1100 => new Pointer<byte>(Ctx, ArgNum),
			_ => throw new ArgumentException($"Invalid type {type} for 8 bit number"),
		};
	}

	public class Any32 : Argument<uint>
	{
		public Any32()
		{
		}

		public Any32(Context ctx, int type) : base(ctx, type)
		{
		}

		public override Type[] AssignableFrom => [typeof(Immediate<uint>), typeof(Register), typeof(Pointer<uint>)];

		public override IArgument Create(Context ctx, int argnum) => new Any32(ctx, argnum);

		public override string Dissassemble() => GetArg(Ctx.Data[ArgNum]).Dissassemble();
		public override uint Read() => GetArg(Ctx.Data[ArgNum]).Read();
		public override void Write(uint value) => GetArg(Ctx.Data[ArgNum]).Write(value);

		private Argument<uint> GetArg(byte type) => type switch
		{
			0b0000 => new Immediate<uint>(Ctx, ArgNum),
			0b0001 => new DoubleRegister(Ctx, ArgNum),
			0b0010 or 0b0011 or 0b0100 or 0b0101 or 0b0110 or 0b0111 or 0b1000 or 0b1001 or 0b1010 or 0b1011 or 0b1100 => new Pointer<uint>(Ctx, ArgNum),
			_ => throw new ArgumentException($"Invalid type {type} for 32 bit any argument"),
		};
	}
}
