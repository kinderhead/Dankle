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
	public class Builder(IDriver driver)
	{
		public readonly IDriver Driver = driver;

		private readonly List<string> progs = [];

		public void AddSourceFiles(params string[] paths)
		{
			foreach (var i in paths)
			{
				Driver.Compile(i, "../CTest/tmp.obj");
				var asm = Driver.Dissassemble("../CTest/tmp.obj");
				var tokenizer = new IntelTokenizer(asm);
				var tokens = tokenizer.Parse();
				var parser = new IntelParser(tokens);
				parser.Parse();
				progs.Add(parser.Output);
			}
		}

		public void LinkAndRun()
		{
			var computer = new Computer(0xF0000u);
			computer.AddComponent<Terminal>(0xFFFFFFF0u);

			var linker = new Linker([File.ReadAllText("cmain.asm"), ..progs]);
			computer.WriteMem(0x10000u, linker.AssembleAndLink(0x10000u, computer));
			computer.GetComponent<CPUCore>().ProgramCounter = linker.Symbols["init"];

			computer.Run();
		}
	}
}
