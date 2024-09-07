using Dankle;
using Dankle.Components;

namespace DankleTest
{
	[TestClass]
	public class MemoryTest
	{
		public static Computer GetComputer()
		{
			var computer = new Computer(100000);
			return computer;
		}

		[TestMethod]
		public void TestReadWrite()
		{
			var computer = GetComputer();
			computer.WriteMem<byte>(0, 5);
			Assert.AreEqual(5, computer.ReadMem(0));
		}

		[TestMethod]
		public void TestReadWrite16()
		{
			var computer = GetComputer();
			computer.WriteMem<ushort>(15, ushort.MaxValue - 1);
			Assert.AreEqual(ushort.MaxValue - 1, computer.ReadMem<ushort>(15));
		}

		[TestMethod]
		public void TestEndianness()
		{
			var computer = GetComputer();
			computer.WriteMem<ushort>(15, ushort.MaxValue - 1);
			Assert.AreEqual(0xFF, computer.ReadMem(15));
			Assert.AreEqual(0xFE, computer.ReadMem(16));

			computer.WriteMem(15, computer.ReadMem<ushort>(15));
			Assert.AreEqual(0xFF, computer.ReadMem(15));
			Assert.AreEqual(0xFE, computer.ReadMem(16));
		}
	}
}
