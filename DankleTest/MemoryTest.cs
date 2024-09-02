using Dankle;
using Dankle.Components;

namespace DankleTest
{
	[TestClass]
	public class MemoryTest
	{
		public static Computer GetComputer()
		{
			var computer = new Computer();
			computer.AddComponent<Memory>(100000);
			computer.Run(false);
			return computer;
		}

		[TestMethod]
		public void TestReadWrite()
		{
			var computer = GetComputer();

			var mem = computer.GetComponent<Memory>();
			mem.Write(0, 5);
			Assert.AreEqual(5, mem.Read(0));

			computer.Stop();
		}
	}
}
