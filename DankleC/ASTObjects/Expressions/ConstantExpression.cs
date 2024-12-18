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
		public override ResolvedExpression AsCasted(TypeSpecifier type) => ChangeType(type);

		public override void WriteToRegisters(int[] regs, IRBuilder builder)
		{
			if (Math.Ceiling(Type.Size / 2.0) != regs.Length) throw new InvalidOperationException("Mismatched expression write");

			var words = GetWords();
			for (int i = 0; i < words.Length; i++)
			{
				builder.Add(new LoadImmToReg(regs[i], words[i]));
			}
		}

		public override void WriteToPointer(IPointer pointer, IRBuilder builder)
		{
			throw new NotImplementedException();
		}

		public ushort[] GetWords()
		{
			var words = new List<ushort>();
			var t = (BuiltinTypeSpecifier)Type;

			if (t.Type == BuiltinType.UnsignedInt)
			{
				var val = Convert.ToUInt32(Value);
				words.Add((ushort)(val >>> 16));
				words.Add((ushort)(val & 0xFFFF));
			}
			else if (t.Type == BuiltinType.SignedInt)
			{
				var val = Convert.ToInt32(Value);
				words.Add((ushort)(val >>> 16));
				words.Add((ushort)(val & 0xFFFF));
			}
			else if (t.Type == BuiltinType.SignedShort)
			{
				var val = Convert.ToInt16(Value);
				words.Add((ushort)val);
			}
			else throw new NotImplementedException();

			return [.. words];
		}
	}
}
