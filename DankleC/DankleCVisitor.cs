using System;
using Antlr4.Runtime.Misc;
using DankleC.ASTObjects;

namespace DankleC
{
    public class DankleCVisitor : CBaseVisitor<IASTObject>
    {
        public override IASTObject VisitTranslationUnit([NotNull] CParser.TranslationUnitContext context)
        {
            var program = new ProgramNode();

            foreach (var i in context.children)
            {
                var def = Visit(i);
                if (def is FunctionNode func) program.Functions.Add(func);
            }

            return program;
        }

        public override IASTObject VisitFunctionDefinition([NotNull] CParser.FunctionDefinitionContext context)
        {
            var type = (TypeSpecifier)Visit(context.declarationSpecifiers());
            var decl = (FunctionDeclarator)Visit(context.declarator());
        }

        public override IASTObject VisitDeclarator([NotNull] CParser.DeclaratorContext context)
        {
            var pointer = context.pointer() is not null;
            var dd = context.directDeclarator();

            if (dd.Identifier)
        }
    }
}
