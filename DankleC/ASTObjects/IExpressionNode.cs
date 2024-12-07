using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public interface IExpressionNode : IASTObject
	{
		public TypeSpecifier? GetTypeSpecifier();
	}

	public class ResolvedExpressionNode(TypeSpecifier type) : IExpressionNode
	{
		public readonly TypeSpecifier Type = type;

		public TypeSpecifier? GetTypeSpecifier() => Type;
	}

	public class ConstantExpressionNode(TypeSpecifier type, object value) : ResolvedExpressionNode(type)
	{
		public readonly object Value = value;
	}
}
