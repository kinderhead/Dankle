using DankleC.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects.Expressions
{
	public class StringLiteralExpression(string text, TypeSpecifier type) : ResolvedExpression(type)
	{
		public readonly string Text = text;

		public override bool IsSimpleExpression => true;

		public override ResolvedExpression ChangeType(TypeSpecifier type) => new StringLiteralExpression(Text, type);

		public override IValue Execute(IRBuilder builder)
		{
			var literal = new StringLiteral(Text);
			var label = builder.Add(literal);
			return new LabelVariable(label.Name, Type, builder.CurrentScope);
		}

		public override void Walk(Action<ResolvedExpression> cb)
		{
			cb(this);
		}
	}
}
