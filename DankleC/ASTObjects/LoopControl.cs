using System;
using DankleC.IR;

namespace DankleC.ASTObjects
{
    public class ContinueStatement : Statement
    {
        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            if (builder.CurrentScope.LoopNext is not IRLogicLabel l) throw new InvalidOperationException("Invalid place for a continue statment");
            builder.Add(new IRJump(l));
        }
    }

    public class BreakStatement : Statement
    {
        public override void BuildIR(IRBuilder builder, IRFunction func)
        {
            if (builder.CurrentScope.LoopEnd is not IRLogicLabel l) throw new InvalidOperationException("Invalid place for a continue statment");
            builder.Add(new IRJump(l));
        }
    }
}
