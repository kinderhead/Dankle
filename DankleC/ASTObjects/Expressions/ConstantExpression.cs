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
			if (pointer.Size != Type.Size) throw new InvalidOperationException();

			var words = GetWords();
			for (int i = 0; i < words.Length; i++)
			{
				builder.Add(new LoadImmToReg(8, words[i]));
				if (Type.Size == 1) builder.Add(new LoadRegToPtr8(pointer.Get(i * 2), 8));
				else builder.Add(new LoadRegToPtr(pointer.Get(i * 2), 8));
			}
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
			else if (t.Type == BuiltinType.UnsignedShort)
			{
				var val = Convert.ToUInt16(Value);
				words.Add(val);
			}
			else if (t.Type == BuiltinType.SignedShort)
			{
				var val = Convert.ToInt16(Value);
				words.Add((ushort)val);
			}
			else if (t.Type == BuiltinType.SignedChar)
			{
				var val = Convert.ToSByte(Value);
				words.Add((ushort)((ushort)val & 0xFF)); // Silly C# sign extension
			}
			else if (t.Type == BuiltinType.UnsignedChar)
			{
				var val = Convert.ToByte(Value);
				words.Add(val);
			}
			else throw new NotImplementedException();

			return [.. words];
		}
	}
}
