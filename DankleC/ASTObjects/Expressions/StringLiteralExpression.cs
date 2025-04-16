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

    public class ListInitializer(List<IExpression> values) : UnresolvedExpression
    {
		public readonly List<IExpression> Values = values;

		public override ResolvedExpression Resolve(IRBuilder builder)
		{
			throw new NotImplementedException();
		}
	}

	public class ResolvedListInitializer(List<ResolvedExpression> values, TypeSpecifier type) : ResolvedExpression(type), IToBytes
	{
		public readonly List<ResolvedExpression> Values = values;
		public override bool IsSimpleExpression => true;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new ResolvedListInitializer(Values, type);
		public override IValue Execute(IRBuilder builder) => throw new NotImplementedException();
		public IByteLike ToBytes(IRBuilder builder)
		{
			if (Type is ArrayTypeSpecifier arr)
			{
				if (arr.Size != Values.Count) throw new NotImplementedException();
				return new ConstantArray([.. Values.Select(i => ((IToBytes)i.Cast(arr.Inner)).ToBytes(builder))]);
			}
			else throw new NotImplementedException();
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);

			foreach (var i in Values)
			{
				i.Walk(cb);
			}
		}
	}
}
