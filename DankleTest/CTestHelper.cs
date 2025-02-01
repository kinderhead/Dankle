using System;
using System.Numerics;
using Assembler;
using Dankle;
using Dankle.Components;
using DankleC;
using DankleC.ASTObjects;
using DankleC.IR;

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
                return Computer.ReadMem<T>((uint)(Computer.MainCore.StackPointer + stackvar.Pointer.Offset));
            }
            else throw new NotImplementedException();
        }

		public void Dispose()
		{
            RunUntilDone();
			GC.SuppressFinalize(this);
		}
	}
}
