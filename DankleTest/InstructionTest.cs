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
			Assert.AreEqual(4u, core.ProgramCounter);
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
			Assert.AreEqual(4u, core.ProgramCounter);
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
			computer.WriteMem<ushort>(0, 5);
			computer.WriteMem<ushort>(2, 0x3400);
			computer.WriteMem<ushort>(4, 0);
			computer.WriteMem<ushort>(6, 50);
			core.Step();
			Assert.AreEqual(15, computer.ReadMem<byte>(50));
		}

		[TestMethod]
		public void TestMOV()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[4] = 15;
			computer.WriteMem<ushort>(0, 6);
			computer.WriteMem<ushort>(2, 0x7400);
			core.Step();
			Assert.AreEqual(15, core.Registers[7]);
		}

		[TestMethod]
		public void TestADD()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = ushort.MaxValue;
			core.Registers[1] = 2;
			computer.WriteMem<ushort>(0, 7);
			computer.WriteMem<ushort>(2, 0x0120);
			core.Step();

			Assert.AreEqual(1, core.Registers[2]);
			Assert.IsFalse(core.Zero);
			Assert.IsTrue(core.Overflow);
		}

		[TestMethod]
		public void TestSUB()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = ushort.MaxValue;
			core.Registers[1] = 2;
			computer.WriteMem<ushort>(0, 8);
			computer.WriteMem<ushort>(2, 0x0120);
			core.Step();

			Assert.AreEqual(ushort.MaxValue - 2, core.Registers[2]);
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestSMUL()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = unchecked((ushort)-5);
			core.Registers[1] = 5;
			computer.WriteMem<ushort>(0, 9);
			computer.WriteMem<ushort>(2, 0x0120);
			core.Step();

			Assert.AreEqual(-25, (short)core.Registers[2]);
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestSDIV()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = unchecked((ushort)-5);
			core.Registers[1] = 5;
			computer.WriteMem<ushort>(0, 10);
			computer.WriteMem<ushort>(2, 0x0120);
			core.Step();

			Assert.AreEqual(-1, (short)core.Registers[2]);
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestUMUL()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = 5;
			core.Registers[1] = 0;
			computer.WriteMem<ushort>(0, 11);
			computer.WriteMem<ushort>(2, 0x0120);
			core.Step();

			Assert.AreEqual(0, core.Registers[2]);
			Assert.IsTrue(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestUDIV()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = 5;
			core.Registers[1] = 2;
			computer.WriteMem<ushort>(0, 12);
			computer.WriteMem<ushort>(2, 0x0120);
			core.Step();

			Assert.AreEqual(2, core.Registers[2]);
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestLSH()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = 0b0000100000010000;
			core.Registers[1] = 1;
			computer.WriteMem<ushort>(0, 13);
			computer.WriteMem<ushort>(2, 0x0120);
			core.Step();

			Assert.AreEqual(0b0001000000100000, core.Registers[2]);
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestRSH()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = 0b0000100000010000;
			core.Registers[1] = 1;
			computer.WriteMem<ushort>(0, 14);
			computer.WriteMem<ushort>(2, 0x0120);
			core.Step();

			Assert.AreEqual(0b0000010000001000, core.Registers[2]);
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		// TODO: Write comparison tests but I doubt they don't work

		[TestMethod]
		public void TestJump()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[0] = ushort.MaxValue;
			core.Registers[1] = ushort.MaxValue - 1;
			computer.WriteMem<ushort>(0, 20);
			computer.WriteMem<ushort>(2, 0x1000);
			computer.WriteMem<byte>(4, 0x01);
			core.Step();

			Assert.AreEqual(0xFFFFFFFEu, core.ProgramCounter);
		}

		[TestMethod]
		public void TestProtections()
		{
			using var computer = GetComputer();
			var core = computer.GetComponent<CPUCore>();

			core.Registers[1] = 0x1234;
			computer.WriteMem<ushort>(0, 36);
			computer.WriteMem<ushort>(2, 0x1000);
			computer.WriteMem<ushort>(4, 37);
			computer.WriteMem<ushort>(6, 0x1000);
			computer.WriteMem<ushort>(8, 38);
			computer.WriteMem<ushort>(10, 0x1000);

			core.Step();
			Assert.AreEqual(0x34, core.Registers[1]);
			core.Step();
			Assert.AreEqual(0x12, core.Registers[1]);
			core.Step();
			Assert.AreEqual(0x1234, core.Registers[1]);
		}
	}
}
