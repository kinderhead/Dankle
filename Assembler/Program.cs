using System.Text;
using Dankle;
using Dankle.Components;

namespace Assembler
{
	public class Program
	{
		static void Main()
		{
			var prog = Encoding.Default.GetString(File.ReadAllBytes("../../../prog.asm"));

			using var computer = new Computer(0xFFFF);
			//computer.Debug = true;
			computer.AddComponent<Terminal>(0xFFFFFFF0u);
			computer.AddComponent<Dankle.Components.Timer>(0xFFFFFFF1u);

			var tokenizer = new Tokenizer(prog);
			var parser = new Parser(tokenizer.Parse(), 0x7FFF, computer);
			parser.Parse();
			
			computer.WriteMem(0x7FFF, parser.GetBinary());
			computer.GetComponent<CPUCore>().ProgramCounter = 0x7FFF;
			computer.Run();
			//computer.GetComponent<CPUCore>().Dump();
		}
	}
}
