using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public interface IGlobalDefinition
    {
        public void Handle(IRBuilder builder);
    }
}
