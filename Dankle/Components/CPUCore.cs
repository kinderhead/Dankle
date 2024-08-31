using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class CPUCore(Computer computer) : Component(computer)
	{
		public override string Name => "CPUCore";

		protected override void Process()
		{
			while (true)
			{
				Console.WriteLine("Gaming");
				Thread.Sleep(1000);
			}
		}
	}
}
