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
            var stream = new AntlrFileStream("../../../../CTest/test.c");
            var lexer = new CLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CParser(tokenStream);
            var visitor = new DankleCVisitor();
            var ret = visitor.Visit(parser.root());
            var ir = new IRBuilder((ProgramNode)ret);
            ir.Build();
            var cg = new CodeGen(ir);
            Console.WriteLine(cg.Compile());
        }
    }
}
