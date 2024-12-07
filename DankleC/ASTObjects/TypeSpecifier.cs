using System;

namespace DankleC.ASTObjects
{
    public class TypeSpecifier : IASTObject
    {
        public bool IsPointer;
    }

    public class BuiltinTypeSpecifier(string type) : TypeSpecifier
    {
        public readonly string Type = type;
    }
}
