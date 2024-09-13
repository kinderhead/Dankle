using Dankle.Components;
using Dankle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleTest
{
    [TestClass]
    public class ALUTest
    {
		public static Computer GetComputer()
		{
			var computer = new Computer(69);
			computer.GetComponent<CPUCore>().ShouldStep = true;
			return computer;
		}

		[TestMethod]
		public void TestAdd()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(7, core.ALU.Calculate<ushort>(3, Operation.ADD, 4));
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestSubtract()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(65535, core.ALU.Calculate<ushort>(3, Operation.SUB, 4));
			Assert.IsFalse(core.Zero);
			Assert.IsTrue(core.Overflow);
		}

		[TestMethod]
		public void TestMultiply()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(-45, core.ALU.Calculate<short>(-3, Operation.MUL, 15));
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestDivide()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(5, core.ALU.Calculate<ushort>(20, Operation.DIV, 4));
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestModulo()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(0, core.ALU.Calculate<ushort>(20, Operation.MOD, 4));
			Assert.IsTrue(core.Zero);
		}

		[TestMethod]
		public void TestLeftShift()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(0b00000010, core.ALU.Shift<byte>(0b00000001, ShiftOperation.LSH, 1));
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);

			Assert.AreEqual(0b00000000, core.ALU.Shift<byte>(0b01000000, ShiftOperation.LSH, 2));
			Assert.IsTrue(core.Zero);
			Assert.IsTrue(core.Overflow);
		}

		[TestMethod]
		public void TestRightShift()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(0b00000000, core.ALU.Shift<byte>(0b00000001, ShiftOperation.RSH, 1));
			Assert.IsTrue(core.Zero);
			Assert.IsTrue(core.Overflow);

			Assert.AreEqual(0b00010000, core.ALU.Shift<byte>(0b01000000, ShiftOperation.RSH, 2));
			Assert.IsFalse(core.Zero);
			Assert.IsFalse(core.Overflow);
		}

		[TestMethod]
		public void TestOr()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(0b00000101, core.ALU.Bitwise<byte>(0b00000001, BitwiseOperation.OR, 0b00000100));
			Assert.IsFalse(core.Zero);
		}

		[TestMethod]
		public void TestAnd()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(0b00000000, core.ALU.Bitwise<byte>(0b00000001, BitwiseOperation.AND, 0b00000100));
			Assert.IsTrue(core.Zero);
		}

		[TestMethod]
		public void TestXor()
		{
			var core = GetComputer().GetComponent<CPUCore>();

			Assert.AreEqual(0b00000100, core.ALU.Bitwise<byte>(0b00000001, BitwiseOperation.XOR, 0b00000101));
			Assert.IsFalse(core.Zero);
		}
	}
}
