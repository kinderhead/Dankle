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
		public void TestCharVar()
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
		public void TestUCharVar()
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
		public void TestShortVar()
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
		public void TestUShortVar()
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
		public void TestIntVar()
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
		public void TestUIntVar()
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

		#endregion

		#region Math

		[TestMethod, TestCategory("Math")]
		public void TestCharMath() => CTestHelper.TestMath<sbyte>();

		[TestMethod, TestCategory("Math")]
		public void TestUCharMath() => CTestHelper.TestMath<byte>();

		[TestMethod, TestCategory("Math")]
		public void TestShortMath() => CTestHelper.TestMath<short>();

		[TestMethod, TestCategory("Math")]
		public void TestUShortMath() => CTestHelper.TestMath<ushort>();

		[TestMethod, TestCategory("Math")]
		public void TestIntMath() => CTestHelper.TestMath<int>();

		[TestMethod, TestCategory("Math")]
		public void TestUIntMath() => CTestHelper.TestMath<uint>();

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

		[TestMethod, TestCategory("Casting")]
		public void TestCharCast() => CTestHelper.TestCast<sbyte>();

		[TestMethod, TestCategory("Casting")]
		public void TestUCharCast() => CTestHelper.TestCast<byte>();

		[TestMethod, TestCategory("Casting")]
		public void TestShortCast() => CTestHelper.TestCast<short>();

		[TestMethod, TestCategory("Casting")]
		public void TestUShortCast() => CTestHelper.TestCast<ushort>();

		[TestMethod, TestCategory("Casting")]
		public void TestIntCast() => CTestHelper.TestCast<int>();

		[TestMethod, TestCategory("Casting")]
		public void TestUIntCast() => CTestHelper.TestCast<uint>();

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

		[TestMethod]
		[TestCategory("Pointers")]
		public void TestIntPtrSet()
		{
			using var c = new CTestHelper(@"
short main()
{
    int x = 5;
    int* ptr = &x;
    *ptr = 69;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(69, c.GetVariable<int>("x"));
		}

		[TestMethod]
		[TestCategory("Pointers")]
		public void TestIntArrSet()
		{
			using var c = new CTestHelper(@"
short main()
{
    short i = 4;
    int x[10];
    x[i * 2] = 19;
    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(4, c.GetVariable<short>("i"));
			Assert.AreEqual(19, c.IndexArray<int>("x", 8));
		}

		[TestMethod]
		[TestCategory("Pointers")]
		public void TestShortArrSetGetMath()
		{
			using var c = new CTestHelper(@"
short main()
{
    short x[4];
    x[0] = 10;
    x[1] = 340;
    x[2] = 891;
    x[3] = 50;

    short y = x[0] / x[1] + x[2] * x[3];

    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(10, c.IndexArray<short>("x", 0));
			Assert.AreEqual(340, c.IndexArray<short>("x", 1));
			Assert.AreEqual(891, c.IndexArray<short>("x", 2));
			Assert.AreEqual(50, c.IndexArray<short>("x", 3));
			Assert.AreEqual(unchecked((short)(10 / 340 + 891 * 50)), c.GetVariable<short>("y"));
		}

		#endregion

		#region Comparison

		[TestMethod, TestCategory("Comparison")]
		public void TestCharCompare() => CTestHelper.TestComparison<sbyte>();

		[TestMethod, TestCategory("Comparison")]
		public void TestUCharCompare() => CTestHelper.TestComparison<byte>();

		[TestMethod, TestCategory("Comparison")]
		public void TestShortCompare() => CTestHelper.TestComparison<short>();

		[TestMethod, TestCategory("Comparison")]
		public void TestUShortCompare() => CTestHelper.TestComparison<ushort>();

		[TestMethod, TestCategory("Comparison")]
		public void TestIntCompare() => CTestHelper.TestComparison<int>();

		[TestMethod, TestCategory("Comparison")]
		public void TestUIntCompare() => CTestHelper.TestComparison<uint>();

		[TestMethod, TestCategory("Comparison")]
		public void TestCharLogic() => CTestHelper.TestLogic<sbyte>();

		[TestMethod, TestCategory("Comparison")]
		public void TestUCharLogic() => CTestHelper.TestLogic<byte>();

		[TestMethod, TestCategory("Comparison")]
		public void TestShortLogic() => CTestHelper.TestLogic<short>();

		[TestMethod, TestCategory("Comparison")]
		public void TestUShortLogic() => CTestHelper.TestLogic<ushort>();

		[TestMethod, TestCategory("Comparison")]
		public void TestIntLogic() => CTestHelper.TestLogic<int>();

		[TestMethod, TestCategory("Comparison")]
		public void TestUIntLogic() => CTestHelper.TestLogic<uint>();
		
		[TestMethod]
		[TestCategory("Comparison")]
		public void TestIf()
		{
			using var c = new CTestHelper(@"
short main()
{
    short x = 69;
    if (x == 69) x = 4;

    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(4, c.GetVariable<short>("x"));
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
