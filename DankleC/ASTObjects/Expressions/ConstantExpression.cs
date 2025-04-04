using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class ConstantExpression(TypeSpecifier type, object value) : ResolvedExpression(type)
	{
		public readonly object Value = value;

        public override bool IsSimpleExpression => true;

		public override ResolvedExpression ChangeType(TypeSpecifier type)
		{
			if (type is BuiltinTypeSpecifier b && b.Type == BuiltinType.Bool) return new ConstantExpression(type, (dynamic)Value == 0 ? 0 : 1);
			return new ConstantExpression(type, Value);
		}

		protected override ResolvedExpression AsCasted(TypeSpecifier type) => ChangeType(type);
		public bool IsTrue => (dynamic)Value == 0;

		public override IValue Execute(IRBuilder builder)
		{
			var t = (BuiltinTypeSpecifier)Type;

			if (t.Type == BuiltinType.UnsignedLong)
			{
				if (Value is Int128 i) return new Immediate64(ulong.CreateTruncating(i), t.Type);
				var val = Convert.ToUInt64(Value);
				return new Immediate64(val, t.Type);
			}
			else if (t.Type == BuiltinType.SignedLong)
			{
				if (Value is Int128 i) return new Immediate64((ulong)long.CreateTruncating(i), t.Type);
				var val = Convert.ToInt64(Value);
				return new Immediate64((ulong)val, t.Type);
			}
			else if (t.Type == BuiltinType.UnsignedInt)
			{
				if (Value is Int128 i) return new Immediate32(uint.CreateTruncating(i), t.Type);
				var val = Convert.ToUInt32(Value);
				return new Immediate32(val, t.Type);
			}
			else if (t.Type == BuiltinType.SignedInt)
			{
				if (Value is Int128 i) return new Immediate32((uint)int.CreateTruncating(i), t.Type);
				var val = Convert.ToInt32(Value);
				return new Immediate32((uint)val, t.Type);
			}
			else if (t.Type == BuiltinType.UnsignedShort)
			{
				if (Value is Int128 i) return new Immediate(ushort.CreateTruncating(i), t.Type);
				var val = Convert.ToUInt16(Value);
				return new Immediate(val, t.Type);
			}
			else if (t.Type == BuiltinType.SignedShort)
			{
				if (Value is Int128 i) return new Immediate((ushort)short.CreateTruncating(i), t.Type);
				var val = Convert.ToInt16(Value);
				return new Immediate((ushort)val, t.Type);
			}
			else if (t.Type == BuiltinType.SignedChar)
			{
				if (Value is Int128 i) return new Immediate((ushort)((ushort)sbyte.CreateTruncating(i) & 0xFF), t.Type);
				var val = Convert.ToSByte(Value);
				return new Immediate((ushort)((ushort)val & 0xFF), t.Type); // Silly C# sign extension
			}
			else if (t.Type == BuiltinType.UnsignedChar)
			{
				if (Value is Int128 i) return new Immediate(byte.CreateTruncating(i), t.Type);
				var val = Convert.ToByte(Value);
				return new Immediate(val, t.Type);
			}
			else if (t.Type == BuiltinType.Bool)
			{
				if (Value is Int128 i) return new Immediate(byte.CreateTruncating(i) == 0 ? (byte)0 : (byte)1, t.Type);
				var val = Convert.ToByte(Value);
				return new Immediate(val == 0 ? (byte)0 : (byte)1, t.Type);
			}
			else throw new NotImplementedException();
		}

		public override void Conditional(IRBuilder builder, bool negate = false)
		{
			throw new NotImplementedException("Caller does not handle constant conditionals");
			// var cond = (dynamic)Value != 0;
			// if (negate) cond = !cond;

			// // TODO: Maybe do this better
			// if (cond) builder.Add(new IREq(new Immediate(1, BuiltinType.UnsignedChar), new Immediate(1, BuiltinType.UnsignedChar), false));
			// else builder.Add(new IREq(new Immediate(0, BuiltinType.UnsignedChar), new Immediate(1, BuiltinType.UnsignedChar), false));
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
		}
	}
}
