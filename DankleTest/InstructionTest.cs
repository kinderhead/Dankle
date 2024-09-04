using Dankle;
using Dankle.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleTest
{
	[TestClass]
	public class InstructionTest
	{
		public static Computer GetComputer()
		{
			var computer = new Computer(100000);
			computer.GetComponent<CPUCore>().ShouldStep = true;
			computer.Run(false);
			return computer;
		}

		[TestMethod]
		public void TestHalt()
		{
			var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			computer.WriteMem<byte>(0, 0);
			core.Step();
			Assert.AreEqual(4, core.ProgramCounter);
		}
	}
}
