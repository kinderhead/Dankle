using System;

namespace DankleC.ASTObjects
{
    public class FunctionNode(string name, TypeSpecifier returnType) : IASTObject
    {
        public readonly string Name = name;
        public readonly TypeSpecifier ReturnType = returnType;
    }

    public class FunctionDeclarator(string name, bool isPointer) : IASTObject
    {
        public readonly string Name = name;
        public readonly bool IsPointer = isPointer;
    }
}
