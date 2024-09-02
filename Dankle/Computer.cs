using Dankle.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle
{
	public class Computer
	{
		private readonly List<Component> Components = [];

		public Computer()
		{
			AddComponent<CPUCore>();
		}

		public void AddComponent<T>(params object[] args) where T : Component
		{
			var comp = (T?)Activator.CreateInstance(typeof(T), [this, ..args]) ?? throw new ArgumentException("Could not create component");
			AddComponent(comp);
		}

		public void AddComponent(Component comp)
		{
			Components.Add(comp);
		}

		public T GetComponent<T>() where T : Component
		{
			foreach (var i in Components)
			{
				if (i is T comp) return comp;
			}

			throw new ArgumentException($"Could not find object with type {typeof(T).Name}");
		}

		public void Run(bool blockThread = true)
		{
			foreach (var i in Components)
			{
				i.Run();
			}

			if (blockThread)
			{
				foreach (var i in Components)
				{
					i.WaitUntilFinish();
				}
			}
		}
	}
}
