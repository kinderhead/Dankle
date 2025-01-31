using System;
using Assembler;
using Dankle;
using Dankle.Components;
using DankleC;
using DankleC.ASTObjects;

namespace DankleTest
{
    public class CTestHelper
    {
        public readonly Compiler Compiler;
        public readonly Computer Computer;
        public readonly Linker Linker;

        public CTestHelper(string program)
        {
            Compiler = new Compiler().ReadText(program).GenAST().GenIR(true);
            Computer = new Computer(0xF0000u);
			Computer.AddComponent<Terminal>(0xFFFFFFF0u);
			Computer.AddComponent<Debugger>(0xFFFFFFF2u);

			Linker = new Linker([File.ReadAllText("cmain.asm"), Compiler.GenAssembly()]);
			Computer.WriteMem(0x10000u, Linker.AssembleAndLink(0x10000u, Computer));
			Computer.GetComponent<CPUCore>().ProgramCounter = Linker.Symbols["cmain"];

            Computer.Run(true);
        }

        public void RunUntil<T>(int index = 0) where T : Statement
        {
            var stmt = Compiler.AST.FindAll<T>()[index];
            var bp = Linker.Symbols[$"stmt_{stmt.ID}"];

            while (Computer.MainCore.ProgramCounter != bp)
            {
                Computer.MainCore.Step();
            }
        }
    }
}
