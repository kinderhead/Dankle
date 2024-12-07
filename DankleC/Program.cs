using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace DankleC
{
    public static class Program
    {
        static void Main(string[] _)
        {
            var stream = new AntlrFileStream("../../../../CTest/test.c");
            var lexer = new CLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CParser(tokenStream);
            var visitor = new DankleCVisitor();
            var ret = visitor.Visit(parser.root());
            Console.WriteLine(ret);
        }
    }
}
