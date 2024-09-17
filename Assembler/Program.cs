namespace Assembler
{
	public class Program
	{
		static void Main()
		{
			var prog = @"
test:
	ld r0, 69
";

			var tokenizer = new Tokenizer(prog);
			var parser = new Parser(tokenizer.Parse());
			parser.Parse();
		}
	}
}
