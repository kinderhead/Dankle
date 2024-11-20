using Assembler;
using Dankle;
using Dankle.Components;

namespace DankleTranslator
{
    public static class Program
    {
        public static void Main()
        {
            var driver = new WatcomDriver("wdis");
            var asm = driver.Dissassemble("../CTest/test.obj");
            var tokenizer = new IntelTokenizer(asm);
            var tokens = tokenizer.Parse();
            var parser = new IntelParser(tokens);
            parser.Parse();

            Console.WriteLine(parser.Output);

            var computer = new Computer(0xF0000u);
            computer.AddComponent<Terminal>(0xFFFFFFF0u);

            var linker = new Linker([File.ReadAllText("cmain.asm"), parser.Output]);
            computer.WriteMem(0x10000u, linker.AssembleAndLink(0x10000u, computer));
            computer.GetComponent<CPUCore>().ProgramCounter = linker.Symbols["init"];

            computer.Run();
        }
    }
}
