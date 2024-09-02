using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle
{
	public interface IMessage
	{

	}

	public abstract class Message<TOut>
	{
		public TaskCompletionSource<TOut> Output = new();
		public required Component Source;
	}
}
