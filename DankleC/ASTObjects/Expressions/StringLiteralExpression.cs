using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class StringLiteralExpression(string text, TypeSpecifier type) : ResolvedExpression(type), IToBytes
	{
		public readonly string Text = text;

		public override bool IsSimpleExpression => true;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new StringLiteralExpression(Text, type);

		public override IValue Execute(IRBuilder builder) => new LabelVariable(((StringVariable)ToBytes(builder)).Value, Type, builder.CurrentScope);

		public IByteLike ToBytes(IRBuilder builder)
		{
			var literal = new StringLiteral(Text);
			var label = builder.Add(literal);
			return new StringVariable(label.Name);
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
		}
	}

    public class ConstantArrayExpression(List<IExpression> values, TypeSpecifier type) : ResolvedExpression(type), IToBytes
    {
		public readonly List<IExpression> Values = values;
        public override bool IsSimpleExpression => true;

        public override ResolvedExpression ChangeType(TypeSpecifier type) => new ConstantArrayExpression(Values, type);
		public override IValue Execute(IRBuilder builder) => throw new NotImplementedException();
		public IByteLike ToBytes(IRBuilder builder)
		{
			if (((ArrayTypeSpecifier)Type).Size != Values.Count) throw new NotImplementedException();
			return new ConstantArray([.. Values.Select(i => ((IToBytes)i.Cast(((ArrayTypeSpecifier)Type).Inner)).ToBytes(builder))]);
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
			
			// There should never be any call expressions here and I can't be bothered to make an unresolved and resolved version of
			// ConstantArrayExpression because there's no other reason to.
		}
    }
}
