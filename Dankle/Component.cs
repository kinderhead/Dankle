using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle
{
	public abstract class Component
	{
		public Computer Computer { get; protected set; }
		public Thread Thread { get; protected set; }
		public abstract string Name { get; }

		private readonly BlockingCollection<IMessage> Messages = new(new ConcurrentQueue<IMessage>());
		private readonly Dictionary<Type, Action<IMessage>> Handlers = [];

		public Component(Computer computer)
		{
			Thread = new(Process);
			Computer = computer;
			Init();
		}

		public void Run()
		{
			Thread.Start();
		}

		public void WaitUntilFinish()
		{
			Thread.Join();
		}

		protected virtual void Init() { }

		protected virtual void Process()
		{
			while (true)
			{
				var msg = Messages.Take();
				Handlers[msg.GetType()](msg);
			}
		}

		protected void RegisterHandler<T, TOut>(Func<T, TOut> handler) where T : Message<TOut>
		{
			Handlers[typeof(T)] = (i) =>
			{
				if (i is T msg) msg.Output.SetResult(handler(msg));
				else throw new ArgumentException($"Invalid message handler for {typeof(T).Name}");
			};
		}

		protected TOut Send<T, TOut>(T msg) where T : Message<TOut>
		{
			Messages.Add((IMessage)msg);
			return msg.Output.Task.Result;
		}
	}
}
