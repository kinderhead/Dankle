using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public interface IStatementNode : IASTObject
	{
	}

	public class ReturnStatement(IExpressionNode expression) : IStatementNode
	{
		public readonly IExpressionNode Expression = expression;
	}
}
