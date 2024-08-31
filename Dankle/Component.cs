using System;
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

		public Component(Computer computer)
		{
			Thread = new(Process);
			Computer = computer;
		}

		public void Run()
		{
			Thread.Start();
		}

		protected abstract void Process();
	}
}
