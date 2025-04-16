using System;
using System.Numerics;
using System.Text;
using Assembler;
using Dankle;
using Dankle.Components;
using DankleC;
using DankleC.ASTObjects;
using DankleC.ASTObjects.Expressions;
using DankleC.IR;
using Newtonsoft.Json.Linq;

namespace DankleTest
{
	public class CTestHelper : IDisposable
	{
		public readonly Compiler Compiler;
		public readonly Computer Computer;
		public readonly Linker Linker;

		public readonly StringBuilder Output = new();

		private Statement? CurrentStatement;

		public CTestHelper(string program, bool libc = false)
		{
			Compiler = new Compiler().ReadTextAndPreprocess(program).GenAST().GenIR(true);
			Computer = new Computer(0xF0000u);
			Computer.AddComponent<Terminal>(0xFFFFFFF0u, Output);
			Computer.AddComponent<Debugger>(0xFFFFFFF2u);
			Computer.AddMemoryMapEntry(new RAM(0xFFFF0000, 0xFFFFA000)); // Stack

			Linker = new Linker([new("cmain.asm", File.ReadAllText("cmain.asm")), new("asm", Compiler.GenAssembly()), ..libc ? Compiler.CompileLibC() : []]);
			Computer.WriteMem(0x10000u, Linker.AssembleAndLink(0x10000u, Computer));
			Computer.GetComponent<CPUCore>().ProgramCounter = Linker.Symbols["cmain"];

			Computer.MainCore.ShouldStep = true;
			Computer.Run(false);
		}

		public void RunUntil<T>(int index = 0) where T : Statement
		{
			CurrentStatement = Compiler.AST.FindAll<T>()[index];
			var bp = Linker.Parsers[1].GetVariable<uint>($"stmt_{CurrentStatement.ID}");

			do
			{
				Computer.MainCore.Step();
			}
			while (Computer.MainCore.ProgramCounter != bp); // Make sure it steps at least once
		}

		public void RunUntilDone()
		{
			Computer.Stop();
		}

		public T GetVariable<T>(string name) where T : IBinaryInteger<T>
		{
			if (CurrentStatement is null) throw new InvalidOperationException("Call RunUntil before GetVariable");
			var v = CurrentStatement.Scope.GetVariable(name);

			if (v.Type.Size != TypeInfo<T>.Size) throw new InvalidOperationException("Wrong type");

			if (v is RegisterVariable regvar)
			{
				List<byte> data = [];
				foreach (var i in regvar.Registers)
				{
					if (regvar.Type.Size != 1) data.Add((byte)((Computer.MainCore.Registers[i] & 0xFF00) >> 8));
					data.Add((byte)Computer.MainCore.Registers[i]);
				}
				return Utils.FromBytes<T>([.. data]);
			}
			else if (v is PointerVariable stackvar)
			{
				return Computer.ReadMem<T>((uint)(Computer.MainCore.StackPointer + CurrentStatement.Scope.MaxFuncAllocStackUsed + ((StackPointer)stackvar.Pointer).Offset));
			}
			else throw new NotImplementedException();
		}

		public T IndexArray<T>(string name, int index) where T : IBinaryInteger<T>
		{
			if (CurrentStatement is null) throw new InvalidOperationException("Call RunUntil before GetVariable");
			var v = CurrentStatement.Scope.GetVariable(name);
			var type = ((ArrayTypeSpecifier)v.Type).Inner;

			if (type.Size != TypeInfo<T>.Size) throw new InvalidOperationException("Wrong type");

			if (v is PointerVariable stackvar)
			{
				return Computer.ReadMem<T>((uint)(Computer.MainCore.StackPointer + CurrentStatement.Scope.MaxFuncAllocStackUsed + ((StackPointer)stackvar.Pointer).Offset + (index * type.Size)));
			}
			else throw new NotImplementedException();
		}

		public void Dispose()
		{
			RunUntilDone();
			GC.SuppressFinalize(this);
		}

		public static void TestMathAdd<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.CreateTruncating(1), T.MaxValue, ArithmeticOperation.Addition);
			TestMath(T.MinValue, T.MaxValue, ArithmeticOperation.Addition);
			TestMath(T.MaxValue / T.CreateTruncating(2), T.MaxValue / T.CreateTruncating(4) * T.CreateTruncating(3), ArithmeticOperation.Addition);
		}

		public static void TestMathSub<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.MinValue, T.CreateTruncating(1), ArithmeticOperation.Subtraction);
			TestMath(T.MaxValue, T.MinValue, ArithmeticOperation.Subtraction);
			TestMath(T.MaxValue / T.CreateTruncating(2), T.MaxValue / T.CreateTruncating(4) * T.CreateTruncating(3), ArithmeticOperation.Subtraction);
		}

		public static void TestMathMul<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.CreateTruncating(2), (T.MaxValue / T.CreateTruncating(2)) - T.CreateTruncating(1), ArithmeticOperation.Multiplication);
			TestMath(T.MinValue, T.MaxValue, ArithmeticOperation.Multiplication);
		}

		public static void TestMathDiv<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.MaxValue, T.CreateTruncating(2), ArithmeticOperation.Division);
			TestMath(T.MinValue, T.CreateTruncating(2), ArithmeticOperation.Division);
		}

		public static void TestMathMod<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.MaxValue, T.CreateTruncating(3), ArithmeticOperation.Modulo);
			TestMath(T.MinValue, T.CreateTruncating(3), ArithmeticOperation.Modulo);

			TestMath(T.MaxValue, T.CreateTruncating(127), ArithmeticOperation.Modulo);
			TestMath(T.MinValue, T.CreateTruncating(127), ArithmeticOperation.Modulo);
		}

		public static void TestMathLeftShift<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.MaxValue, T.CreateTruncating(3), ArithmeticOperation.LeftShift);
			TestMath(T.MinValue + T.One, T.CreateTruncating(2), ArithmeticOperation.LeftShift);
			TestMath(T.One, T.One, ArithmeticOperation.LeftShift);
		}

		public static void TestMathRightShift<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.MaxValue, T.CreateTruncating(3), ArithmeticOperation.RightShift);
			TestMath(T.MinValue + T.One, T.CreateTruncating(2), ArithmeticOperation.RightShift);
			TestMath(T.CreateTruncating(2), T.One, ArithmeticOperation.RightShift);
		}

		public static void TestMathInclusiveOr<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.AllBitsSet, T.AllBitsSet / T.CreateTruncating(3), ArithmeticOperation.InclusiveOr);
		}

		public static void TestMathExclusiveOr<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.AllBitsSet, T.AllBitsSet / T.CreateTruncating(3), ArithmeticOperation.ExclusiveOr);
		}

		public static void TestMathAnd<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.AllBitsSet, T.AllBitsSet / T.CreateTruncating(3), ArithmeticOperation.And);
		}

		public static void TestMath<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMathAdd<T>();
			TestMathSub<T>();
			TestMathMul<T>();
			TestMathDiv<T>();
			TestMathMod<T>();
			TestMathLeftShift<T>();
			TestMathRightShift<T>();
			TestMathInclusiveOr<T>();
			TestMathExclusiveOr<T>();
			TestMathAnd<T>();
		}

		public static void TestMath<T>(T x, T y, ArithmeticOperation op) where T : IBinaryInteger<T>
		{
			var opchar = op switch
			{
				ArithmeticOperation.Addition => "+",
				ArithmeticOperation.Subtraction => "-",
				ArithmeticOperation.Multiplication => "*",
				ArithmeticOperation.Division => "/",
				ArithmeticOperation.Modulo => "%",
				ArithmeticOperation.LeftShift => "<<",
				ArithmeticOperation.RightShift => ">>",
				ArithmeticOperation.InclusiveOr => "|",
				ArithmeticOperation.ExclusiveOr => "^",
				ArithmeticOperation.And => "&",
				_ => throw new InvalidOperationException(),
			};

			var type = CUtils.NumberTypeToString<T>();

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = {y};
	{type} z = x {opchar} y;
    return 0;
}}
");
			T z = op switch
			{
				ArithmeticOperation.Addition => x + y,
				ArithmeticOperation.Subtraction => x - y,
				ArithmeticOperation.Multiplication => x * y,
				ArithmeticOperation.Division => x / y,
				ArithmeticOperation.Modulo => x % y,
				ArithmeticOperation.LeftShift => x << int.CreateTruncating(y),
				ArithmeticOperation.RightShift => x >> int.CreateTruncating(y),
				ArithmeticOperation.InclusiveOr => x | y,
				ArithmeticOperation.ExclusiveOr => x ^ y,
				ArithmeticOperation.And => x & y,
				_ => throw new InvalidOperationException(),
			};

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(y, c.GetVariable<T>("y"));
			Assert.AreEqual(z, c.GetVariable<T>("z"));

			TestMathAssign(x, y, op);
		}

		public static void TestMathAssign<T>(T x, T y, ArithmeticOperation op) where T : IBinaryInteger<T>
		{
			var opchar = op switch
			{
				ArithmeticOperation.Addition => "+",
				ArithmeticOperation.Subtraction => "-",
				ArithmeticOperation.Multiplication => "*",
				ArithmeticOperation.Division => "/",
				ArithmeticOperation.Modulo => "%",
				ArithmeticOperation.LeftShift => "<<",
				ArithmeticOperation.RightShift => ">>",
				ArithmeticOperation.InclusiveOr => "|",
				ArithmeticOperation.ExclusiveOr => "^",
				ArithmeticOperation.And => "&",
				_ => throw new InvalidOperationException(),
			};

			var type = CUtils.NumberTypeToString<T>();

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = {y};
	x {opchar}= y;
    return 0;
}}
");
			T z = op switch
			{
				ArithmeticOperation.Addition => x + y,
				ArithmeticOperation.Subtraction => x - y,
				ArithmeticOperation.Multiplication => x * y,
				ArithmeticOperation.Division => x / y,
				ArithmeticOperation.Modulo => x % y,
				ArithmeticOperation.LeftShift => x << int.CreateTruncating(y),
				ArithmeticOperation.RightShift => x >> int.CreateTruncating(y),
				ArithmeticOperation.InclusiveOr => x | y,
				ArithmeticOperation.ExclusiveOr => x ^ y,
				ArithmeticOperation.And => x & y,
				_ => throw new InvalidOperationException(),
			};

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(z, c.GetVariable<T>("x"));
			Assert.AreEqual(y, c.GetVariable<T>("y"));
		}

		public static void TestCast<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestCast<T, sbyte>(T.MinValue + T.CreateTruncating(1));
			TestCast<T, byte>(T.MinValue + T.CreateTruncating(1));
			TestCast<T, short>(T.MinValue + T.CreateTruncating(1));
			TestCast<T, ushort>(T.MinValue + T.CreateTruncating(1));
			TestCast<T, int>(T.MinValue + T.CreateTruncating(1));
			TestCast<T, uint>(T.MinValue + T.CreateTruncating(1));
			TestCast<T, long>(T.MinValue + T.CreateTruncating(1));
			TestCast<T, ulong>(T.MinValue + T.CreateTruncating(1));

			TestCast<T, sbyte>(T.MaxValue - T.CreateTruncating(1));
			TestCast<T, byte>(T.MaxValue - T.CreateTruncating(1));
			TestCast<T, short>(T.MaxValue - T.CreateTruncating(1));
			TestCast<T, ushort>(T.MaxValue - T.CreateTruncating(1));
			TestCast<T, int>(T.MaxValue - T.CreateTruncating(1));
			TestCast<T, uint>(T.MaxValue - T.CreateTruncating(1));
			TestCast<T, long>(T.MaxValue - T.CreateTruncating(1));
			TestCast<T, ulong>(T.MaxValue - T.CreateTruncating(1));
		}

		public static void TestCast<T, R>(T x) where T : IBinaryInteger<T> where R : IBinaryInteger<R>
		{
			var type1 = CUtils.NumberTypeToString<T>();
			var type2 = CUtils.NumberTypeToString<R>();

			using var c = new CTestHelper(@$"
short main()
{{
    {type1} x = {x};
	{type2} y = x;
    return 0;
}}
");

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(R.CreateTruncating(x), c.GetVariable<R>("y"));
		}

		public static void TestComparison<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var x = T.MaxValue - T.CreateTruncating(1);
			var y = T.MinValue + T.CreateTruncating(1);

			TestComparison(x, y, EqualityOperation.Equals);
			TestComparison(x, y, EqualityOperation.NotEquals);
			TestComparison(x, x, EqualityOperation.Equals);
			TestComparison(x, x, EqualityOperation.NotEquals);
			TestComparison(y, y, EqualityOperation.Equals);
			TestComparison(y, y, EqualityOperation.NotEquals);

			TestComparison(x, y, EqualityOperation.LessThan);
			TestComparison(y, x, EqualityOperation.LessThan);
			TestComparison(x, x, EqualityOperation.LessThanOrEqual);
			TestComparison(y, y, EqualityOperation.LessThanOrEqual);
			TestComparison(x, y, EqualityOperation.LessThanOrEqual);
			TestComparison(y, x, EqualityOperation.LessThanOrEqual);

			TestComparison(x, y, EqualityOperation.GreaterThan);
			TestComparison(y, x, EqualityOperation.GreaterThan);
			TestComparison(x, x, EqualityOperation.GreaterThanOrEqual);
			TestComparison(y, y, EqualityOperation.GreaterThanOrEqual);
			TestComparison(x, y, EqualityOperation.GreaterThanOrEqual);
			TestComparison(y, x, EqualityOperation.GreaterThanOrEqual);
		}

		public static void TestComparison<T>(T x, T y, EqualityOperation op) where T : IBinaryInteger<T>
		{
			var opchar = op switch
			{
				EqualityOperation.Equals => "==",
				EqualityOperation.NotEquals => "!=",
				EqualityOperation.LessThan => "<",
				EqualityOperation.LessThanOrEqual => "<=",
				EqualityOperation.GreaterThan => ">",
				EqualityOperation.GreaterThanOrEqual => ">=",
				_ => throw new InvalidOperationException(),
			};

			var type = CUtils.NumberTypeToString<T>();

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = {y};
	char z = x {opchar} y;
    return 0;
}}
");
			bool z = op switch
			{
				EqualityOperation.Equals => x == y,
				EqualityOperation.NotEquals => x != y,
				EqualityOperation.LessThan => x < y,
				EqualityOperation.LessThanOrEqual => x <= y,
				EqualityOperation.GreaterThan => x > y,
				EqualityOperation.GreaterThanOrEqual => x >= y,
				_ => throw new InvalidOperationException(),
			};

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(y, c.GetVariable<T>("y"));
			Assert.AreEqual(z ? 1 : 0, c.GetVariable<byte>("z"));
		}

		public static void TestLogic<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var x = T.MaxValue - T.CreateTruncating(1);
			var y = T.MinValue + T.CreateTruncating(1);

			TestLogic(x, y, T.Zero, LogicalOperation.And);
			TestLogic(x, x, T.Zero, LogicalOperation.And);
			TestLogic(x, y, T.One, LogicalOperation.And);
			TestLogic(y, y, T.One, LogicalOperation.And);

			TestLogic(x, y, T.Zero, LogicalOperation.Or);
			TestLogic(x, x, T.Zero, LogicalOperation.Or);
			TestLogic(x, y, T.One, LogicalOperation.Or);
			TestLogic(y, y, T.One, LogicalOperation.Or);

			TestNot<T>();
		}

		public static void TestLogic<T>(T x, T y, T z, LogicalOperation op) where T : IBinaryInteger<T>
		{
			var opchar = op switch
			{
				LogicalOperation.And => "&&",
				LogicalOperation.Or => "||",
				_ => throw new InvalidOperationException(),
			};

			var type = CUtils.NumberTypeToString<T>();

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = {y};
	{type} z = {z};
	char a = x == y {opchar} z;
    return 0;
}}
");
			bool a = op switch
			{
				LogicalOperation.And => x == y && z != T.Zero,
				LogicalOperation.Or => x == y || z != T.Zero,
				_ => throw new InvalidOperationException(),
			};

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(y, c.GetVariable<T>("y"));
			Assert.AreEqual(z, c.GetVariable<T>("z"));
			Assert.AreEqual(a ? 1 : 0, c.GetVariable<byte>("a"));
		}

		public static void TestPostIncrement<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestPostIncrement<T>(false);
			TestPostIncrement<T>(true);
		}

		public static void TestPostIncrement<T>(bool decrement) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();
			var x = decrement ? T.MinValue : T.MaxValue;

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = x{(decrement ? "--" : "++")};
    return 0;
}}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x + (decrement ? -T.One : T.One), c.GetVariable<T>("x"));
			Assert.AreEqual(x, c.GetVariable<T>("y"));
		}

		public static void TestPreIncrement<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestPreIncrement<T>(false);
			TestPreIncrement<T>(true);
		}

		public static void TestPreIncrement<T>(bool decrement) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();
			var x = decrement ? T.MinValue : T.MaxValue;

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = {(decrement ? "--" : "++")}x;
    return 0;
}}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x + (decrement ? -T.One : T.One), c.GetVariable<T>("x"));
			Assert.AreEqual(x + (decrement ? -T.One : T.One), c.GetVariable<T>("y"));
		}

		public static void TestNegate<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();
			var x = T.MaxValue / T.CreateTruncating(4) * T.CreateTruncating(3);

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = -x;
    return 0;
}}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(-x, c.GetVariable<T>("y"));
		}

		public static void TestNot<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestNot(T.MaxValue / T.CreateTruncating(4) * T.CreateTruncating(3));
			TestNot(T.Zero);
		}

		public static void TestNot<T>(T x) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = !x;
    return 0;
}}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(x == T.Zero ? T.One : T.Zero, c.GetVariable<T>("y"));
		}

		public static void TestBitwiseNot<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestBitwiseNot(T.MaxValue / T.CreateTruncating(4) * T.CreateTruncating(3));
			TestBitwiseNot(T.Zero);
			TestBitwiseNot(T.AllBitsSet);
		}

		public static void TestBitwiseNot<T>(T x) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();

			using var c = new CTestHelper(@$"
short main()
{{
    {type} x = {x};
	{type} y = ~x;
    return 0;
}}
");
			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(~x, c.GetVariable<T>("y"));
		}

		public static void TestSimpleFunction<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();
			var x = T.MaxValue;

			using var c = new CTestHelper(@$"
{type} test()
{{
	return {x};
}}

short main()
{{
    {type} x = test();
    return 0;
}}
");
			c.RunUntil<ReturnStatement>(1);
			Assert.AreEqual(x, c.GetVariable<T>("x"));
		}

		public static void TestFunctionOneArg<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();
			var x = T.MaxValue - T.One;

			using var c = new CTestHelper(@$"
{type} test({type} x)
{{
	int _ = 0;
	return x + 1;
}}

short main()
{{
	int _ = 0;
    {type} x = test({x});
    return 0;
}}
");
			c.RunUntil<ReturnStatement>(1);
			Assert.AreEqual(x + T.One, c.GetVariable<T>("x"));
		}

		public static void TestFunctionTwoArg<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();
			var x = T.MaxValue - T.One;
			var y = T.One;

			using var c = new CTestHelper(@$"
{type} test({type} x, {type} y)
{{
	int _ = 0;
	return x + y;
}}

short main()
{{
	int _ = 0;
    {type} x = test({x}, {y});
    return 0;
}}
");
			c.RunUntil<ReturnStatement>(1);
			Assert.AreEqual(x + T.One, c.GetVariable<T>("x"));
		}

		public static void TestFunctionTwoArgNested<T>() where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var type = CUtils.NumberTypeToString<T>();
			var a = T.One;
			var b = T.MaxValue / T.CreateTruncating(4);
			var c = T.MaxValue / T.CreateTruncating(2);
			var d = T.MaxValue / T.CreateTruncating(4) * T.CreateTruncating(3);

			using var cpu = new CTestHelper(@$"
{type} test({type} x, {type} y)
{{
	{type} ret = x + y;
	return ret;
}}

short main()
{{
	int _ = 0;
    {type} x = test(test({a}, {b}), test({c}, {d}));
    return 0;
}}
");
			cpu.RunUntil<ReturnStatement>(0);
			Assert.AreEqual(a + b, cpu.GetVariable<T>("ret"));
			cpu.RunUntil<ReturnStatement>(0);
			Assert.AreEqual(c + d, cpu.GetVariable<T>("ret"));

			cpu.RunUntil<ReturnStatement>(1);
			Assert.AreEqual(a + b + c + d, cpu.GetVariable<T>("x"));
		}
	}
}
