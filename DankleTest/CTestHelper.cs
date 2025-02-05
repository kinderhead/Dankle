using System;
using System.Numerics;
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

        private Statement? CurrentStatement;

        public CTestHelper(string program)
        {
            Compiler = new Compiler().ReadText(program).GenAST().GenIR(true);
            Computer = new Computer(0xF0000u);
			Computer.AddComponent<Terminal>(0xFFFFFFF0u);
			Computer.AddComponent<Debugger>(0xFFFFFFF2u);

			Linker = new Linker([File.ReadAllText("cmain.asm"), Compiler.GenAssembly()]);
			Computer.WriteMem(0x10000u, Linker.AssembleAndLink(0x10000u, Computer));
			Computer.GetComponent<CPUCore>().ProgramCounter = Linker.Symbols["cmain"];

            Computer.MainCore.ShouldStep = true;
            Computer.Run(false);
        }

        public void RunUntil<T>(int index = 0) where T : Statement
        {
			CurrentStatement = Compiler.AST.FindAll<T>()[index];
            var bp = Linker.Parsers[1].GetVariable<uint>($"stmt_{CurrentStatement.ID}");

            while (Computer.MainCore.ProgramCounter != bp)
            {
                Computer.MainCore.Step();
            }
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
            else if (v is StackVariable stackvar)
            {
                return Computer.ReadMem<T>((uint)(Computer.MainCore.StackPointer + ((StackPointer)stackvar.Pointer).Offset));
            }
            else throw new NotImplementedException();
        }

		public T IndexArray<T>(string name, int index) where T : IBinaryInteger<T>
		{
			if (CurrentStatement is null) throw new InvalidOperationException("Call RunUntil before GetVariable");
            var v = CurrentStatement.Scope.GetVariable(name);
			var type = ((ArrayTypeSpecifier)v.Type).Inner;

            if (type.Size != TypeInfo<T>.Size) throw new InvalidOperationException("Wrong type");

			if (v is StackVariable stackvar)
            {
                return Computer.ReadMem<T>((uint)(Computer.MainCore.StackPointer + ((StackPointer)stackvar.Pointer).Offset + (index * type.Size)));
            }
            else throw new NotImplementedException();
		}

		public void Dispose()
		{
            RunUntilDone();
			GC.SuppressFinalize(this);
		}

		public static void TestMathAdd<T>(bool stack) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.CreateTruncating(1), T.MaxValue, stack, ArithmeticOperation.Addition);
			TestMath(T.MinValue, T.MaxValue, stack, ArithmeticOperation.Addition);
		}

		public static void TestMathSub<T>(bool stack) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.MinValue, T.CreateTruncating(1), stack, ArithmeticOperation.Subtraction);
			TestMath(T.MaxValue, T.MinValue, stack, ArithmeticOperation.Subtraction);
		}

		public static void TestMathMul<T>(bool stack) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.CreateTruncating(2), (T.MaxValue / T.CreateTruncating(2)) - T.CreateTruncating(1), stack, ArithmeticOperation.Multiplication);
			TestMath(T.MinValue, T.MaxValue, stack, ArithmeticOperation.Multiplication);
		}

		public static void TestMathDiv<T>(bool stack) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMath(T.MaxValue, T.CreateTruncating(2), stack, ArithmeticOperation.Division);
			TestMath(T.MinValue, T.CreateTruncating(2), stack, ArithmeticOperation.Division);
		}

		public static void TestMath<T>(bool stack) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			TestMathAdd<T>(stack);
			TestMathSub<T>(stack);
			TestMathMul<T>(stack);
			TestMathDiv<T>(stack);
		}

		public static void TestMath<T>(T x, T y, bool stack, ArithmeticOperation op) where T : IBinaryInteger<T>
        {
			var opchar = op switch
			{
				ArithmeticOperation.Addition => "+",
				ArithmeticOperation.Subtraction => "-",
				ArithmeticOperation.Multiplication => "*",
				ArithmeticOperation.Division => "/",
				_ => throw new InvalidOperationException(),
			};

			var type = CUtils.NumberTypeToString<T>();

			using var c = new CTestHelper(@$"
short main()
{{
	{(stack ? "int _1 = 0; int _2 = 0;" : "")}
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
				_ => throw new InvalidOperationException(),
			};

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(y, c.GetVariable<T>("y"));
			Assert.AreEqual(z, c.GetVariable<T>("z"));
		}

		public static void TestCast<T>(bool stack) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			T x = TypeInfo<T>.IsUnsigned ? T.MaxValue : T.MinValue;

			TestCast<T, sbyte>(x, stack);
			TestCast<T, byte>(x, stack);
			TestCast<T, short>(x, stack);
			TestCast<T, ushort>(x, stack);
			TestCast<T, int>(x, stack);
			TestCast<T, uint>(x, stack);
		}

		public static void TestCast<T, R>(T x, bool stack) where T : IBinaryInteger<T> where R : IBinaryInteger<R>
		{
			var type1 = CUtils.NumberTypeToString<T>();
			var type2 = CUtils.NumberTypeToString<R>();

			using var c = new CTestHelper(@$"
short main()
{{
	{(stack ? "int _1 = 0; int _2 = 0;" : "")}
    {type1} x = {x};
	{type2} y = x;
    return 0;
}}
");

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(R.CreateTruncating(x), c.GetVariable<R>("y"));
		}

		public static void TestComparaison<T>(bool stack) where T : IBinaryInteger<T>, IMinMaxValue<T>
		{
			var x = T.MaxValue - T.CreateTruncating(1);
			var y = T.MinValue + T.CreateTruncating(1);

			TestComparaison(x, y, stack, EqualityOperation.Equals);
			//TestComparaison(x, y, stack, EqualityOperation.NotEquals);
		}

		public static void TestComparaison<T>(T x, T y, bool stack, EqualityOperation op) where T : IBinaryInteger<T>
        {
			var opchar = op switch
			{
				EqualityOperation.Equals => "==",
				EqualityOperation.NotEquals => "!=",
				_ => throw new InvalidOperationException(),
			};

			var type = CUtils.NumberTypeToString<T>();

			using var c = new CTestHelper(@$"
short main()
{{
	{(stack ? "int _1 = 0; int _2 = 0;" : "")}
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
				_ => throw new InvalidOperationException(),
			};

			c.RunUntil<ReturnStatement>();
			Assert.AreEqual(x, c.GetVariable<T>("x"));
			Assert.AreEqual(y, c.GetVariable<T>("y"));
			Assert.AreEqual(z ? 1 : 0, c.GetVariable<byte>("z"));
		}
	}
}
