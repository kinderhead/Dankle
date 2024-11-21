using Assembler;
using Dankle;
using Dankle.Components;

namespace DankleTranslator
{
    public static class Program
    {
        public static void Main()
        {
            var driver = new WatcomDriver("wcc", "wdis");
            var builder = new Builder(driver);

            builder.AddSourceFiles("../CTest/test.c", "../CTest/lib.c", "../CTest/printf.c");
            builder.LinkAndRun();
        }
    }
}
