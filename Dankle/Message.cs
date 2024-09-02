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

	public abstract class Message<TOut> : IMessage
	{
		public TaskCompletionSource<TOut> Output = new();
		public Component? Source;
	}

	public class StopMessage : Message<bool>
	{

	}
}
