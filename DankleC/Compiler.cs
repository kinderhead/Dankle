using System;
using Antlr4.Runtime;
using Dankle;
using DankleC.ASTObjects;
using DankleC.IR;

namespace DankleC
{
    public class Compiler
    {
        public ICharStream? Stream { get; private set; }
        public ProgramNode? AST { get; private set; }
        public IRBuilder? IR { get; private set; }

        public Compiler ReadFile(string path)
        {
            Stream = new AntlrFileStream(path);
            return this;
        }

        public Compiler ReadText(string program)
        {
            Stream = new AntlrInputStream(program);
            return this;
        }

        public Compiler GenAST()
        {
            var lexer = new CLexer(Stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CParser(tokenStream);
            var visitor = new DankleCVisitor();
            AST = (ProgramNode)visitor.Visit(parser.root());
            return this;
        }

        public Compiler GenIR(bool debug = false)
        {
            IR = new(AST ?? throw new InvalidOperationException(), debug);
            IR.Build();
            return this;
        }

        public string GenAssembly() => new CodeGen(IR ?? throw new InvalidOperationException()).Compile();

        public string Compile() => GenAST().GenIR().GenAssembly();
    }
}
