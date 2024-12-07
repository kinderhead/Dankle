using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleC.ASTObjects
{
	public class ScopeNode : IASTObject
	{
		public readonly List<IStatementNode> Statements = [];
	}
}
