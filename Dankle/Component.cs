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
		public readonly Guid ID;

		protected bool ShouldStop = false;

		private readonly BlockingCollection<IMessage> Messages = [.. new ConcurrentQueue<IMessage>()];
		private readonly Dictionary<Type, Action<IMessage>> Handlers = [];

		public Component(Computer computer)
		{
			ID = Guid.NewGuid();
			Thread = new(Process);
			Computer = computer;

			RegisterHandler((StopMessage i) =>
			{
				ShouldStop = true;
				return true;
			});
		}

		public void Run()
		{
			Thread.Start();
		}

		public void WaitUntilFinish()
		{
			if (Thread.IsAlive) Thread.Join();
		}

		public void Stop()
		{
			if (ShouldStop || !Thread.IsAlive) return;
			Send<StopMessage, bool>(new StopMessage());
			WaitUntilFinish();
		}

		protected virtual void Process()
		{
			while (!ShouldStop)
			{
				HandleMessage(true);
			}
		}

		protected void HandleMessage(bool block)
		{
			if (block)
			{
				var msg = Messages.Take();
				Handlers[msg.GetType()](msg);
			}
			else if (Messages.TryTake(out var msg))
			{
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
			Messages.Add(msg);
			return msg.Output.Task.Result;
		}
	}
}
