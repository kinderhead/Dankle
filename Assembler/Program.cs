using Dankle;
using Dankle.Components;

namespace Assembler
{
	public class Program
	{
		static void Main()
		{
			var prog = @"
main:
	ld r0, text
	call write
	hlt
write:
	ld r1, 1
	push r0
	push r1
	push r2
write_loop:
	ld r2, [r0]
	cmp [r0], 0
	je write_end

	stb [0x1000], r2
	add r1, r0, r0
	jmp write_loop
write_end:
	pop r2
	pop r1
	pop r0
	ret

text:
	""Me want gaming""
";

			var tokenizer = new Tokenizer(prog);
			var parser = new Parser(tokenizer.Parse());
			parser.Parse();

			using var computer = new Computer(0x0FFF);
			//computer.Debug = true;
			computer.AddComponent<Terminal>(0x1000u);
			computer.WriteMem(0, parser.GetBinary());
			computer.Run();
			computer.GetComponent<CPUCore>().Dump();
			Console.WriteLine(computer.ReadMem<ushort>(69));
		}
	}
}
