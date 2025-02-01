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

		#region Arithmetic

		[TestMethod]
		[TestCategory("Math")]
		[DataRow(false, ArithmeticOperation.Addition)]
		[DataRow(true, ArithmeticOperation.Addition)]
		public void TestCharMath(bool stack, ArithmeticOperation op)
		{
			var opchar = op switch
			{
				ArithmeticOperation.Addition => "+",
				ArithmeticOperation.Multiplication => "*",
				_ => throw new InvalidOperationException(),
			};

			using var c = new CTestHelper(@$"
short main()
{{
	{(stack ? "int _1 = 0; int _2 = 0;" : "")}
    signed char x = 5;
	signed char y = 2;
	signed char z = x {opchar} y;
    return 0;
}}
");
			sbyte x = 5;
			sbyte y = 2;
			sbyte z = op switch
			{
				ArithmeticOperation.Addition => (sbyte)(x + y),
				ArithmeticOperation.Multiplication => (sbyte)(x * y),
				_ => throw new InvalidOperationException(),
			};

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<sbyte>("x"));
			Assert.AreEqual(y, c.GetVariable<sbyte>("y"));
			Assert.AreEqual(z, c.GetVariable<sbyte>("z"));
		}

		#endregion
	}
}
