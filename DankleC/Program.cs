using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Assembler;
using Dankle.Components;
using Dankle;
using DankleC.ASTObjects;
using DankleC.IR;

namespace DankleC
{
    public static class Program
    {
        static void Main(string[] _)
        {
            var compiler = new Compiler();
			var asm = compiler.ReadFileAndPreprocess("../../../../CTest/test.c").GenAST().GenIR().GenAssembly();

			var computer = new Computer(0xF0000u);
			computer.AddComponent<Terminal>(0xFFFFFFF0u);
			computer.AddComponent<Debugger>(0xFFFFFFF2u);

			var libc = Compiler.CompileLibC();
			var linker = new Linker([File.ReadAllText("cmain.asm"), asm, ..libc]);

			Console.WriteLine(asm + "\n" + libc[1] + "\n-----------------------------");

			computer.WriteMem(0x10000u, linker.AssembleAndLink(0x10000u, computer));
			computer.GetComponent<CPUCore>().ProgramCounter = linker.Symbols["cmain"];

			computer.Run();

			Console.WriteLine("");
		}
    }
}
