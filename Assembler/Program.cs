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
	ld r0, text_1
	call write

	ld r0, period
	ldb r1, 0
	ldb r2, 1
	ld r3, 1000
_loop:
	st [0x1001], r3
	call write
	add r1, r2, r1
	cmp r1, 3
	jne _loop

	ld r0, text_2
	call write

	hlt

write:
	push r0
	push r1
	push r2
	ldb r1, 1
write_loop:
	ldb r2, [r0]
	cmp r2, 0
	je write_end

	stb [0x1000], r2
	add r1, r0, r0
	jmp write_loop
write_end:
	pop r2
	pop r1
	pop r0
	ret

period: "".""
text_1: ""Wah""
text_2: ""\nGaming OS\n""
";

			var tokenizer = new Tokenizer(prog);
			var parser = new Parser(tokenizer.Parse());
			parser.Parse();

			using var computer = new Computer(0x0FFF);
			//computer.Debug = true;
			computer.AddComponent<Terminal>(0x1000u);
			computer.AddComponent<Dankle.Components.Timer>(0x1001u);
			computer.WriteMem(0, parser.GetBinary());
			computer.Run();
			computer.GetComponent<CPUCore>().Dump();
		}
	}
}
