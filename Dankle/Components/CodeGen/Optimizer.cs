using Dankle.Components.Instructions;
using System;

namespace Dankle.Components.CodeGen
{
    public readonly struct InsnDef
	{
		public readonly CGInsn? Insn;
		public readonly string? Label;

		public InsnDef(CGInsn insn) => Insn = insn;
		public InsnDef(string label) => Label = label;
	}

    public class Optimizer(Optimizer.Settings settings)
    {
        public readonly Settings OptimizationSettings = settings;

        private List<InsnDef> currentDefs = [];
        private int index = 0;

        public void Optimize(List<InsnDef> defs)
        {
            if (OptimizationSettings.Simple) SimplePass(defs);
        }
        
        private InsnDef? Next() => index + 1 < currentDefs.Count ? currentDefs[index + 1] : null;
        private string? NextIsLabel() => Next() is InsnDef d && d.Label is string label ? label : null;
        private CGInsn? NextIsInsn() => Next() is InsnDef d && d.Insn is CGInsn insn ? insn : null;

        private void Remove()
        {
            currentDefs.RemoveAt(index);
            index--;
        }

        private void SimplePass(List<InsnDef> defs)
        {
            index = 0;
            currentDefs = defs;

            for (; index < currentDefs.Count; index++)
            {
                if (currentDefs[index].Insn is CGInsn insn)
                {
                    CheckRedundantJump(insn);
                    ReduceFCMP(insn);
                    RemoveRedundantInsns(insn);
                }
            }
        }

        private void CheckRedundantJump(CGInsn insn)
        {
            if (insn.Insn is IJumpInsn jmp)
            {
                if (NextIsLabel() is string label && insn.Args[jmp.JumpArgIndex].Equals(new CGLabel<uint>(label))) Remove();
            }
        }

        private void ReduceFCMP(CGInsn insn)
        {
            if (insn.Insn is FlipCompare)
            {
                var e = NextIsInsn();

                if (e?.Insn is JumpEq)
                {
                    e.Insn = new JumpNeq();
                    Remove();
                }
                else if (e?.Insn is JumpNeq)
                {
                    e.Insn = new JumpEq();
                    Remove();
                }
            }
        }

        private void RemoveRedundantInsns(CGInsn insn)
        {
            if (insn.Insn is Move && insn.Args[0].Equals(insn.Args[1])) Remove();
        }

        public readonly record struct Settings(bool Simple, bool Destructive); // Default args don't work for some reason
    }
}
