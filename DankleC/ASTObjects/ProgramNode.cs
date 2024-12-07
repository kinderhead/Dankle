using System;

namespace DankleC.ASTObjects
{
    public class ProgramNode : IASTObject
    {
        public readonly List<FunctionNode> Functions = [];
    }
}
