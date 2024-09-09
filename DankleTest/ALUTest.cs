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
			Assert.IsFalse(core.Overflow);
		}
	}
}
