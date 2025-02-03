using DankleC.ASTObjects;
using DankleC.ASTObjects.Expressions;
using System;

namespace DankleTest
{
    [TestClass]
    public class CTest
    {
		#region Variables
		[TestMethod]
		[TestCategory("Variables")]
        public void TestCharRegVar()
        {
            using var c = new CTestHelper(@"
short main()
{
    signed char x = -5;
    return 0;
}
");
            c.RunUntil<ReturnStatement>();
            Assert.AreEqual(-5, c.GetVariable<sbyte>("x"));
        }

		[TestMethod]
		[TestCategory("Variables")]
		public void TestUCharRegVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    unsigned char x = 255;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(255, c.GetVariable<byte>("x"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void TestShortRegVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    short x = -5;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(-5, c.GetVariable<short>("x"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void TestUShortRegVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    unsigned short x = 65530;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(65530, c.GetVariable<ushort>("x"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void TestIntRegVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    int x = -65530;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(-65530, c.GetVariable<int>("x"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void TestUIntRegVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    unsigned int x = 2147483648;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(2147483648, c.GetVariable<uint>("x"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void Test2ShortVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    short x = 1;
    short y = 2;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(1, c.GetVariable<short>("x"));
			Assert.AreEqual(2, c.GetVariable<short>("y"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void Test2IntVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    int x = 1;
    int y = 2;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(1, c.GetVariable<int>("x"));
			Assert.AreEqual(2, c.GetVariable<int>("y"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void TestCharStackVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    int x = 1;
    int y = 2;
	char z = 3;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(1, c.GetVariable<int>("x"));
			Assert.AreEqual(2, c.GetVariable<int>("y"));
			Assert.AreEqual(3, c.GetVariable<byte>("z"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void TestShortStackVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    int x = 1;
    int y = 2;
	short z = -32768;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(1, c.GetVariable<int>("x"));
			Assert.AreEqual(2, c.GetVariable<int>("y"));
			Assert.AreEqual(-32768, c.GetVariable<short>("z"));
		}

		[TestMethod]
		[TestCategory("Variables")]
		public void TestIntStackVar()
		{
			using var c = new CTestHelper(@"
short main()
{
    int x = 1;
    int y = 2;
	int z = -3276800;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(1, c.GetVariable<int>("x"));
			Assert.AreEqual(2, c.GetVariable<int>("y"));
			Assert.AreEqual(-3276800, c.GetVariable<int>("z"));
		}
		#endregion

		#region Math

		[TestMethod, TestCategory("Math"), DataRow(false), DataRow(true)]
		public void TestCharMath(bool stack) => CTestHelper.TestMath<sbyte>(stack);

		[TestMethod, TestCategory("Math"), DataRow(false), DataRow(true)]
		public void TestUCharMath(bool stack) => CTestHelper.TestMath<byte>(stack);

		[TestMethod, TestCategory("Math"), DataRow(false), DataRow(true)]
		public void TestShortMath(bool stack) => CTestHelper.TestMath<short>(stack);

		[TestMethod, TestCategory("Math"), DataRow(false), DataRow(true)]
		public void TestUShortMath(bool stack) => CTestHelper.TestMath<ushort>(stack);

		[TestMethod, TestCategory("Math"), DataRow(false), DataRow(true)]
		public void TestIntMath(bool stack) => CTestHelper.TestMath<int>(stack);

		[TestMethod, TestCategory("Math"), DataRow(false), DataRow(true)]
		public void TestUIntMath(bool stack) => CTestHelper.TestMath<uint>(stack);

		[TestMethod]
		[TestCategory("Math")]
		public void TestBigIntMath()
		{
			using var computer = new CTestHelper(@"
short main()
{
    int a = 2355;
	int b = 48973;
	int c = -1248;
	int d = -9;

	int x = (a + b) * c / d * (c * c * c + 32) - 400;

    return 0;
}
");
			int a = 2355;
			int b = 48973;
			int c = -1248;
			int d = -9;
			computer.RunUntil<ReturnStatement>();
			Assert.AreEqual((a + b) * c / d * (c * c * c + 32) - 400, computer.GetVariable<int>("x"));
		}

		#endregion

		#region Casting

		[TestMethod, TestCategory("Casting"), DataRow(false), DataRow(true)]
		public void TestCharCast(bool stack) => CTestHelper.TestCast<sbyte>(stack);

		[TestMethod, TestCategory("Casting"), DataRow(false), DataRow(true)]
		public void TestUCharCast(bool stack) => CTestHelper.TestCast<byte>(stack);

		[TestMethod, TestCategory("Casting"), DataRow(false), DataRow(true)]
		public void TestShortCast(bool stack) => CTestHelper.TestCast<short>(stack);

		[TestMethod, TestCategory("Casting"), DataRow(false), DataRow(true)]
		public void TestUShortCast(bool stack) => CTestHelper.TestCast<ushort>(stack);

		[TestMethod, TestCategory("Casting"), DataRow(false), DataRow(true)]
		public void TestIntCast(bool stack) => CTestHelper.TestCast<int>(stack);

		[TestMethod, TestCategory("Casting"), DataRow(false), DataRow(true)]
		public void TestUIntCast(bool stack) => CTestHelper.TestCast<uint>(stack);

		#endregion

		#region Pointers

		[TestMethod]
		[TestCategory("Pointers")]
		public void TestShortPtrGet()
		{
			using var c = new CTestHelper(@"
short main()
{
    short x = 69;
	short* ptr = &x;
	short y = *ptr;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(69, c.GetVariable<short>("x"));
			Assert.AreEqual(69, c.GetVariable<short>("y"));
		}

		#endregion

//		#region Return

//		[TestMethod]
//		[TestCategory("Return")]
//		public void TestReturnChar()
//		{
//			using var c = new CTestHelper(@"
//char main()
//{
//    char x = 10;
//    return x;
//}
//");
//			c.RunUntilDone();
//			Assert.AreEqual(10, c.Computer.MainCore.Registers[0]);
//		}

//		#endregion
	}
}
