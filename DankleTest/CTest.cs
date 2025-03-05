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
		public void CharVar()
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
		public void UCharVar()
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
		public void ShortVar()
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
		public void UShortVar()
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
		public void IntVar()
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
		public void UIntVar()
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
		public void Short2Var()
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
		public void Int2Var()
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
		public void CharMath() => CTestHelper.TestMath<sbyte>();

		[TestMethod, TestCategory("Math")]
		public void UCharMath() => CTestHelper.TestMath<byte>();

		[TestMethod, TestCategory("Math")]
		public void ShortMath() => CTestHelper.TestMath<short>();

		[TestMethod, TestCategory("Math")]
		public void UShortMath() => CTestHelper.TestMath<ushort>();

		[TestMethod, TestCategory("Math")]
		public void IntMath() => CTestHelper.TestMath<int>();

		[TestMethod, TestCategory("Math")]
		public void UIntMath() => CTestHelper.TestMath<uint>();

		[TestMethod, TestCategory("Math")]
		public void LongMath() => CTestHelper.TestMath<long>();

		[TestMethod, TestCategory("Math")]
		public void ULongMath() => CTestHelper.TestMath<ulong>();


		[TestMethod, TestCategory("Math")]
		public void CharPostIncrement() => CTestHelper.TestPostIncrement<sbyte>();

		[TestMethod, TestCategory("Math")]
		public void UCharPostIncrement() => CTestHelper.TestPostIncrement<byte>();

		[TestMethod, TestCategory("Math")]
		public void ShortPostIncrement() => CTestHelper.TestPostIncrement<short>();

		[TestMethod, TestCategory("Math")]
		public void UShortPostIncrement() => CTestHelper.TestPostIncrement<ushort>();

		[TestMethod, TestCategory("Math")]
		public void IntPostIncrement() => CTestHelper.TestPostIncrement<int>();

		[TestMethod, TestCategory("Math")]
		public void UIntPostIncrement() => CTestHelper.TestPostIncrement<uint>();

		[TestMethod, TestCategory("Math")]
		public void CharPreIncrement() => CTestHelper.TestPreIncrement<sbyte>();

		[TestMethod, TestCategory("Math")]
		public void UCharPreIncrement() => CTestHelper.TestPreIncrement<byte>();

		[TestMethod, TestCategory("Math")]
		public void ShortPreIncrement() => CTestHelper.TestPreIncrement<short>();

		[TestMethod, TestCategory("Math")]
		public void UShortPreIncrement() => CTestHelper.TestPreIncrement<ushort>();

		[TestMethod, TestCategory("Math")]
		public void IntPreIncrement() => CTestHelper.TestPreIncrement<int>();

		[TestMethod, TestCategory("Math")]
		public void UIntPreIncrement() => CTestHelper.TestPreIncrement<uint>();

		[TestMethod]
		[TestCategory("Math")]
		public void BigIntMath()
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
		public void CharCast() => CTestHelper.TestCast<sbyte>();

		[TestMethod, TestCategory("Casting")]
		public void UCharCast() => CTestHelper.TestCast<byte>();

		[TestMethod, TestCategory("Casting")]
		public void ShortCast() => CTestHelper.TestCast<short>();

		[TestMethod, TestCategory("Casting")]
		public void UShortCast() => CTestHelper.TestCast<ushort>();

		[TestMethod, TestCategory("Casting")]
		public void IntCast() => CTestHelper.TestCast<int>();

		[TestMethod, TestCategory("Casting")]
		public void UIntCast() => CTestHelper.TestCast<uint>();

		#endregion

		#region Pointers

		[TestMethod]
		[TestCategory("Pointers")]
		public void ShortPtrGet()
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
		public void IntPtrSet()
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
		public void IntArrSet()
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
		public void ShortArrSetGetMath()
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

		#region Logic

		[TestMethod, TestCategory("Logic")]
		public void CharCompare() => CTestHelper.TestComparison<sbyte>();

		[TestMethod, TestCategory("Logic")]
		public void UCharCompare() => CTestHelper.TestComparison<byte>();

		[TestMethod, TestCategory("Logic")]
		public void ShortCompare() => CTestHelper.TestComparison<short>();

		[TestMethod, TestCategory("Logic")]
		public void UShortCompare() => CTestHelper.TestComparison<ushort>();

		[TestMethod, TestCategory("Logic")]
		public void IntCompare() => CTestHelper.TestComparison<int>();

		[TestMethod, TestCategory("Logic")]
		public void UIntCompare() => CTestHelper.TestComparison<uint>();

		[TestMethod, TestCategory("Logic")]
		public void CharLogic() => CTestHelper.TestLogic<sbyte>();

		[TestMethod, TestCategory("Logic")]
		public void UCharLogic() => CTestHelper.TestLogic<byte>();

		[TestMethod, TestCategory("Logic")]
		public void ShortLogic() => CTestHelper.TestLogic<short>();

		[TestMethod, TestCategory("Logic")]
		public void UShortLogic() => CTestHelper.TestLogic<ushort>();

		[TestMethod, TestCategory("Logic")]
		public void IntLogic() => CTestHelper.TestLogic<int>();

		[TestMethod, TestCategory("Logic")]
		public void UIntLogic() => CTestHelper.TestLogic<uint>();

		[TestMethod]
		[TestCategory("Logic")]
		public void If()
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

		[TestMethod]
		[TestCategory("Logic")]
		public void IfElse()
		{
			using var c = new CTestHelper(@"
short main()
{
    short x = 69;
    if (x == 69) x = 4;
	else x = 3;

	if (x == 3) x = -9;
	else x = 2;

    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(2, c.GetVariable<short>("x"));
		}

		[TestMethod]
		[TestCategory("Logic")]
		public void While()
		{
			using var c = new CTestHelper(@"
short main()
{
    short x = 0;

    while (x != 10)
    {
        x = x + 1;
    }

    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(10, c.GetVariable<short>("x"));
		}

		[TestMethod]
		[TestCategory("Logic")]
		public void WhileInt()
		{
			using var c = new CTestHelper(@"
short main()
{
    int x = 0;

    while (x != 10)
    {
        x = x + 1;
    }

    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(10, c.GetVariable<int>("x"));
		}

		[TestMethod]
		[TestCategory("Logic")]
		public void DoWhile()
		{
			using var c = new CTestHelper(@"
short main()
{
    short x = 0;

    do
    {
        x = x + 1;
    }
	while (x != 10);

    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(10, c.GetVariable<short>("x"));
		}

		[TestMethod]
		[TestCategory("Logic")]
		public void ForShort()
		{
			using var c = new CTestHelper(@"
short main()
{
	short x;
    for (short i = 0; i < 10; i++)
    {
        x = i;
    }

    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(9, c.GetVariable<short>("x"));
		}

		[TestMethod]
		[TestCategory("Logic")]
		public void ForInt()
		{
			using var c = new CTestHelper(@"
short main()
{
	int x;
    for (int i = 0; i < 10; i++)
    {
        x = i;
    }

    return 0;
}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(9, c.GetVariable<int>("x"));
		}

		#endregion

		#region Functions

		[TestMethod, TestCategory("Functions")]
		public void CharSimpleFunc() => CTestHelper.TestSimpleFunction<sbyte>();

		[TestMethod, TestCategory("Functions")]
		public void UCharSimpleFunc() => CTestHelper.TestSimpleFunction<byte>();

		[TestMethod, TestCategory("Functions")]
		public void ShortSimpleFunc() => CTestHelper.TestSimpleFunction<short>();

		[TestMethod, TestCategory("Functions")]
		public void UShortSimpleFunc() => CTestHelper.TestSimpleFunction<ushort>();

		[TestMethod, TestCategory("Functions")]
		public void IntSimpleFunc() => CTestHelper.TestSimpleFunction<int>();

		[TestMethod, TestCategory("Functions")]
		public void UIntSimpleFunc() => CTestHelper.TestSimpleFunction<uint>();


		[TestMethod, TestCategory("Functions")]
		public void CharOneArgFunc() => CTestHelper.TestFunctionOneArg<sbyte>();

		[TestMethod, TestCategory("Functions")]
		public void UCharOneArgFunc() => CTestHelper.TestFunctionOneArg<byte>();

		[TestMethod, TestCategory("Functions")]
		public void ShortOneArgFunc() => CTestHelper.TestFunctionOneArg<short>();

		[TestMethod, TestCategory("Functions")]
		public void UShortOneArgFunc() => CTestHelper.TestFunctionOneArg<ushort>();

		[TestMethod, TestCategory("Functions")]
		public void IntOneArgFunc() => CTestHelper.TestFunctionOneArg<int>();

		[TestMethod, TestCategory("Functions")]
		public void UIntOneArgFunc() => CTestHelper.TestFunctionOneArg<uint>();


		[TestMethod, TestCategory("Functions")]
		public void CharTwoArgFunc() => CTestHelper.TestFunctionTwoArg<sbyte>();

		[TestMethod, TestCategory("Functions")]
		public void UCharTwoArgFunc() => CTestHelper.TestFunctionTwoArg<byte>();

		[TestMethod, TestCategory("Functions")]
		public void ShortTwoArgFunc() => CTestHelper.TestFunctionTwoArg<short>();

		[TestMethod, TestCategory("Functions")]
		public void UShortTwoArgFunc() => CTestHelper.TestFunctionTwoArg<ushort>();

		[TestMethod, TestCategory("Functions")]
		public void IntTwoArgFunc() => CTestHelper.TestFunctionTwoArg<int>();

		[TestMethod, TestCategory("Functions")]
		public void UIntTwoArgFunc() => CTestHelper.TestFunctionTwoArg<uint>();
		
		#endregion

		//		#region Return

		//		[TestMethod]
		//		[TestCategory("Return")]
		//		public void ReturnChar()
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
