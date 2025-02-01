using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DankleC.ASTObjects;
using DankleC.IR;

namespace DankleC
{
    public static class Program
    {
        static void Main(string[] _)
        {
            var compiler = new Compiler();
            Console.WriteLine(compiler.ReadFile("../../../../CTest/test.c").GenAST().GenIR().GenAssembly());
            Console.ReadKey();
        }
    }
}
