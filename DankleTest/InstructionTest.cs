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

			computer.WriteMem<ushort>(0, 2);
			computer.WriteMem<ushort>(2, 0x1000);
			computer.WriteMem<ushort>(4, 0xFE68);
			core.Step();
			Assert.AreEqual(0xFE68, core.Registers[1]);
		}

		[TestMethod]
		public void TestST()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[4] = 15;
			computer.WriteMem<ushort>(0, 3);
			computer.WriteMem<ushort>(2, 0x3400);
			computer.WriteMem<ushort>(4, 0);
			computer.WriteMem<ushort>(6, 50);
			core.Step();
			Assert.AreEqual(15, computer.ReadMem<ushort>(50));
		}

		[TestMethod]
		public void TestLD8()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			computer.WriteMem<ushort>(0, 4);
			computer.WriteMem<ushort>(2, 0x5000);
			computer.WriteMem<byte>(4, 0x68);
			core.Step();
			Assert.AreEqual(0x68, core.Registers[5]);
		}

		[TestMethod]
		public void TestST8()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[4] = 15;
			computer.WriteMem<ushort>(0, 3);
			computer.WriteMem<ushort>(2, 0x3400);
			computer.WriteMem<ushort>(4, 0);
			computer.WriteMem<ushort>(6, 50);
			core.Step();
			Assert.AreEqual(15, computer.ReadMem<ushort>(50));
		}
	}
}
