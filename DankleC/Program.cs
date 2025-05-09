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
			OrderedDictionary<string, string> asm = [];
			var computer = new Computer(0xF0000u);

			computer.AddComponent<Terminal>(0xFFFFFFF0u);
			computer.AddComponent<Debugger>(0xFFFFFFF6u);
			computer.AddComponent<Filesystem>(0xFFFFFF00u);
			computer.AddMemoryMapEntry(new RAM(0xFFF00000u, 0xFFFFEFFFu)); // Program
			computer.AddMemoryMapEntry(new RAM(0xFFFF0000u, 0xFFFFA000u)); // Stack

			Linker osLinker;
			byte[] osData;

			using (var pb = new ProgressBar((Compiler.LibC.Length + Compiler.DankleOS.Length) * 2 + 1, "Compiling...", new ProgressBarOptions() { ProgressCharacter = '─', CollapseWhenFinished = true }))
			{
				var compiler = new Compiler();

				foreach (var i in Compiler.DankleOS)
				{
					asm[i] = compiler.ReadFileAndPreprocess(i, pb, "-I\"../../../../DankleOS/include\"").GenAST().GenIR().GenAssembly(pb);
				}

				var libc = Compiler.CompileLibC(pb);
				osLinker = new Linker([Compiler.CMain, .. asm, .. libc]);
				osData = osLinker.AssembleAndLink(0x10000u, computer, pb);
				osLinker.SaveSymbolfile("dankleos.sym");
			}

			{
				var compiler = new Compiler();
				asm["test.c"] = compiler.ReadFileAndPreprocess("../../../../CTest/test.c", null, "-I\"../../../../DankleOS/include\"").GenAST().GenIR().GenAssembly();

				var linker = new Linker([Compiler.ProgMain, new("test.c", asm["test.c"])]);
				linker.LoadSymbolFile("dankleos.sym");
				var data = linker.AssembleAndLink(0xFFF00004u, computer);

				File.WriteAllBytes("fs/prog", [..Utils.ToBytes(linker.Symbols["progmain"]), ..data]);
			}

			computer.WriteMem(0x10000u, osData);
			computer.GetComponent<CPUCore>().ProgramCounter = osLinker.Symbols["cmain"];

			//Console.WriteLine("\n" + asm.First().Value + "\n-----------------------------");
			Console.WriteLine();

			computer.Run();

			Console.WriteLine($"\nLowest SP: 0x{computer.MainCore.LowestSP:X8}");
		}
    }
}
