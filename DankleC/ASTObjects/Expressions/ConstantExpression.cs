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

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ConstantExpression(type, Value);
		protected override ResolvedExpression AsCasted(TypeSpecifier type) => ChangeType(type);

		public override IValue Execute(IRBuilder builder)
		{
			var t = (BuiltinTypeSpecifier)Type;

			if (t.Type == BuiltinType.UnsignedLong)
			{
				var val = Convert.ToUInt64(Value);
				return new Immediate64(val, t.Type);
			}
			else if (t.Type == BuiltinType.SignedLong)
			{
				var val = Convert.ToInt64(Value);
				return new Immediate64((ulong)val, t.Type);
			}
			else if (t.Type == BuiltinType.UnsignedInt)
			{
				var val = Convert.ToUInt32(Value);
				return new Immediate32(val, t.Type);
			}
			else if (t.Type == BuiltinType.SignedInt)
			{
				var val = Convert.ToInt32(Value);
				return new Immediate32((uint)val, t.Type);
			}
			else if (t.Type == BuiltinType.UnsignedShort)
			{
				var val = Convert.ToUInt16(Value);
				return new Immediate(val, t.Type);
			}
			else if (t.Type == BuiltinType.SignedShort)
			{
				var val = Convert.ToInt16(Value);
				return new Immediate((ushort)val, t.Type);
			}
			else if (t.Type == BuiltinType.SignedChar)
			{
				var val = Convert.ToSByte(Value);
				return new Immediate((ushort)((ushort)val & 0xFF), t.Type); // Silly C# sign extension
			}
			else if (t.Type == BuiltinType.UnsignedChar)
			{
				var val = Convert.ToByte(Value);
				return new Immediate(val, t.Type);
			}
			else throw new NotImplementedException();
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
		}
	}
}
