using System.Text;
using Dankle;
using Dankle.Components;

namespace Assembler
{
	public class Program
	{
		static void Main()
		{
			var prog = Encoding.Default.GetString(File.ReadAllBytes("prog.asm"));

			using var computer = new Computer(0x100000);
			//computer.Debug = true;
			computer.AddComponent<Terminal>(0xFFFFFFF0u);
			computer.AddComponent<Dankle.Components.Timer>(0xFFFFFFF1u);

			var tokenizer = new Tokenizer(prog);
			var parser = new Parser(tokenizer.Parse(), 0x10000, computer);
			parser.Parse();
			
			computer.WriteMem(0x10000, parser.GetBinary());
			computer.GetComponent<CPUCore>().ProgramCounter = 0x10000;
			computer.RunDebug();
			//computer.GetComponent<CPUCore>().Dump();
		}
	}
}
