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
		public void TestHLT()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			computer.WriteMem<ushort>(0, 0);
			core.Step();
			Assert.AreEqual(4, core.ProgramCounter);
			Thread.Sleep(10);
			Assert.IsTrue(computer.StoppingOrStopped);
		}

		[TestMethod]
		public void TestNOP()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			computer.WriteMem<ushort>(0, 1);
			core.Step();
			Assert.AreEqual(4, core.ProgramCounter);
		}

		[TestMethod]
		public void TestLD()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[4] = 15;
			computer.WriteMem<ushort>(0, 3);
			computer.WriteMem<ushort>(2, 0x3400);
			computer.WriteMem<ushort>(4, 0xFE68);
			core.Step();
			Assert.AreEqual(0xFE68, comp);
		}
	}
}
