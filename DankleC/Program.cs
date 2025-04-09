using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Assembler;
using Dankle.Components;
using Dankle;
using DankleC.ASTObjects;
using DankleC.IR;
using ShellProgressBar;

namespace DankleC
{
    public static class Program
    {
        static void Main(string[] _)
        {
			string asm;
			var computer = new Computer(0xF0000u);

			using (var pb = new ProgressBar((Compiler.LibC.Length + 1) * 2 + 1, "Compiling...", new ProgressBarOptions() { ProgressCharacter = '─' }))
			{
				var compiler = new Compiler();
				asm = compiler.ReadFileAndPreprocess("../../../../CTest/test.c", pb).GenAST().GenIR().GenAssembly(pb);

				computer.AddComponent<Terminal>(0xFFFFFFF0u);
				computer.AddComponent<Debugger>(0xFFFFFFF2u);
				computer.AddMemoryMapEntry(new RAM(0xFFFF0000, 0xFFFFA000)); // Stack

				var libc = Compiler.CompileLibC(pb);
				var linker = new Linker([new("cmain.asm", File.ReadAllText("cmain.asm")), new("../../../../CTest/test.c", asm), .. libc]);

				computer.WriteMem(0x10000u, linker.AssembleAndLink(0x10000u, computer, pb));
				computer.GetComponent<CPUCore>().ProgramCounter = linker.Symbols["cmain"];
			}

			Console.WriteLine("\n" + asm + "\n-----------------------------");

			computer.Run();

			Console.WriteLine($"\nLowest SP: 0x{computer.MainCore.LowestSP:X8}");
		}
    }
}
