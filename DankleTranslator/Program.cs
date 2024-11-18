namespace DankleTranslator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var driver = new WatcomDriver("/usr/bin/watcom/binl64/wdis");
            Console.WriteLine(driver.Dissassemble("../CTest/test.o"));
        }
    }
}
