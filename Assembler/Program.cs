using Dankle;
using Dankle.Components;

namespace Assembler
{
	public class Program
	{
		static void Main()
		{
			var prog = @"
test:
	ld r0, 69
	jmp label
	hlt
label:
	ld r0, 3
";

			var tokenizer = new Tokenizer(prog);
			var parser = new Parser(tokenizer.Parse());
			parser.Parse();

			using var computer = new Computer(1000);
			computer.WriteMem(0, parser.GetBinary());
			computer.Run();
			computer.GetComponent<CPUCore>().Dump();
			Console.WriteLine(computer.ReadMem<ushort>(69));
		}
	}
}
