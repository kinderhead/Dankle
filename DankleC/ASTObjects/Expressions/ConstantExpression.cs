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

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ConstantExpression(type, Value);
		protected override ResolvedExpression AsCasted(TypeSpecifier type) => ChangeType(type);

		public override IValue Execute()
		{
			var t = (BuiltinTypeSpecifier)Type;

			if (t.Type == BuiltinType.UnsignedInt)
			{
				var val = Convert.ToUInt32(Value);
				throw new NotImplementedException();
				// words.Add((ushort)(val >>> 16));
				// words.Add((ushort)(val & 0xFFFF));
			}
			else if (t.Type == BuiltinType.SignedInt)
			{
				var val = Convert.ToInt32(Value);
				throw new NotImplementedException();
				// words.Add((ushort)(val >>> 16));
				// words.Add((ushort)(val & 0xFFFF));
			}
			else if (t.Type == BuiltinType.UnsignedShort)
			{
				var val = Convert.ToUInt16(Value);
				return new Immediate(val);
			}
			else if (t.Type == BuiltinType.SignedShort)
			{
				var val = Convert.ToInt16(Value);
				return new Immediate((ushort)val);
			}
			else if (t.Type == BuiltinType.SignedChar)
			{
				var val = Convert.ToSByte(Value);
				return new Immediate((ushort)val);
				//words.Add((ushort)((ushort)val & 0xFF)); // Silly C# sign extension
			}
			else if (t.Type == BuiltinType.UnsignedChar)
			{
				var val = Convert.ToByte(Value);
				return new Immediate(val);
			}
			else throw new NotImplementedException();
		}
    }
}
