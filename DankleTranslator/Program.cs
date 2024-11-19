namespace DankleTranslator
{
    public static class Program
    {
        public static void Main()
        {
            var driver = new WatcomDriver("wdis");
            var asm = driver.Dissassemble("../CTest/test.obj");
            var tokenizer = new IntelTokenizer(asm);
            tokenizer.Parse();
            Console.ReadKey();
        }
    }
}
