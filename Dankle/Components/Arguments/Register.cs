using Dankle.Components.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components.Arguments
{
	public class Register : Argument<ushort>
	{
		public override IArgument Create(Context ctx, int argnum) => new Register(ctx, argnum);

		private readonly bool UseArgNumAsRegister = false;

		public override Type[] AssignableFrom => [];

		public Register(Context ctx, int argnum) : base(ctx, argnum)
		{
		}

		public Register(Context ctx, int argnum, bool useArgNumAsRegister) : base(ctx, argnum)
		{
			UseArgNumAsRegister = useArgNumAsRegister;
		}

		public Register()
		{
		}

		public override string Dissassemble() => $"r{(UseArgNumAsRegister ? ArgNum : Ctx.Data[ArgNum])}";
		public override ushort Read() => Ctx.Core.Registers[UseArgNumAsRegister ? ArgNum : Ctx.Data[ArgNum]];
		public override void Write(ushort value) => Ctx.Core.Registers[UseArgNumAsRegister ? ArgNum : Ctx.Data[ArgNum]] = value;
	}

	public class DoubleRegister : Argument<uint>
	{
		public DoubleRegister()
		{
		}

		public DoubleRegister(Context ctx, int argnum) : base(ctx, argnum)
		{
		}

		public override Type[] AssignableFrom => [];

		public override IArgument Create(Context ctx, int argnum) => new DoubleRegister(ctx, argnum);

		public override string Dissassemble()
		{
			var data = Ctx.Core.GetNext<byte>();
			return $"(r{data >>> 4}, r{data & 0xF})";
		}

		public override uint Read()
		{
			var data = Ctx.Core.GetNext<byte>();
			return Utils.Merge(Ctx.Core.Registers[data >>> 4], Ctx.Core.Registers[data & 0xF]);
		}

		public override void Write(uint value)
		{
			var data = Ctx.Core.GetNext<byte>();
			Ctx.Core.Registers[data >>> 4] = (ushort)(value >>> 16);
			Ctx.Core.Registers[data & 0xF] = (ushort)value;
		}
	}

	public class QuadRegister : Argument<ulong>
	{
		public QuadRegister()
		{
		}

		public QuadRegister(Context ctx, int argnum) : base(ctx, argnum)
		{
		}

		public override Type[] AssignableFrom => [];

		public override IArgument Create(Context ctx, int argnum) => new DoubleRegister(ctx, argnum);

		public override string Dissassemble()
		{
			var data = Ctx.Core.GetNext<ushort>();
			return $"(r{(data >>> 12) & 0xF}, r{(data >>> 8) & 0xF}, r{(data >>> 4) & 0xF}, r{data & 0xF})";
		}

		public override ulong Read()
		{
			var data = Ctx.Core.GetNext<ushort>();
			return Utils.Merge(Utils.Merge(Ctx.Core.Registers[(data >>> 12) & 0xF], Ctx.Core.Registers[(data >>> 8) & 0xF]), Utils.Merge(Ctx.Core.Registers[(data >>> 4) & 0xF], Ctx.Core.Registers[data & 0xF]));
		}

		public override void Write(ulong value)
		{
			var data = Ctx.Core.GetNext<ushort>();
			Ctx.Core.Registers[(data >>> 12) & 0xF] = (ushort)(value >>> 48);
			Ctx.Core.Registers[(data >>> 8) & 0xF] = (ushort)(value >>> 32);
			Ctx.Core.Registers[(data >>> 4) & 0xF] = (ushort)(value >>> 16);
			Ctx.Core.Registers[data & 0xF] = (ushort)value;
		}
	}
}
