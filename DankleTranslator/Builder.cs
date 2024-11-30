using Assembler;
using Dankle.Components;
using Dankle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleTranslator
{
	public class Builder(IDriver driver, string buildDir)
	{
		public readonly IDriver Driver = driver;
		public readonly string BuildDir = buildDir;

		private readonly List<string> progs = [];

		public void AddSourceFiles(params string[] paths)
		{
			Directory.CreateDirectory(BuildDir);

			foreach (var i in paths)
			{
				var objectFile = Path.Join(BuildDir, Path.GetFileNameWithoutExtension(i) + ".obj");

				if (File.GetLastWriteTime(i) > File.GetLastWriteTime(objectFile)) Driver.Compile(i, objectFile);
				var asm = Driver.Dissassemble(objectFile);
				var tokenizer = new IntelTokenizer(asm);
				var tokens = tokenizer.Parse();
				var parser = new IntelParser(tokens);
				parser.Parse();
				progs.Add(parser.Output);

				File.WriteAllText(Path.Join(BuildDir, Path.GetFileNameWithoutExtension(i) + ".asm"), parser.Output);
				File.WriteAllText(Path.Join(BuildDir, Path.GetFileNameWithoutExtension(i) + "_8086.asm"), asm);
			}
		}

		public void LinkAndRun()
		{
			var computer = new Computer(0xF0000u);
			computer.AddComponent<Terminal>(0xFFFFFFF0u);
			computer.AddComponent<Debugger>(0xFFFFFFF2u);

			var linker = new Linker([File.ReadAllText("cmain.asm"), ..progs]);
			computer.WriteMem(0x10000u, linker.AssembleAndLink(0x10000u, computer));
			computer.GetComponent<CPUCore>().ProgramCounter = linker.Symbols["init"];

			foreach (var i in linker.Symbols)
			{
				computer.Symbols[i.Key] = i.Value;
			}

			//computer.Debug = true;

			computer.Run();
		}
	}
}
