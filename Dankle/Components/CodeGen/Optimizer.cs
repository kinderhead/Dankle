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

        public void Optimize(List<InsnDef> defs)
        {
            if (OptimizationSettings.Simple) SimplePass(defs);
        }

        private void SimplePass(List<InsnDef> defs)
        {
            int i = 0;

            InsnDef? next() => i + 1 < defs.Count ? defs[i + 1] : null;
            void remove()
            {
                defs.RemoveAt(i);
                i--;
            }

            for (; i < defs.Count; i++)
            {
                if (defs[i].Insn is CGInsn insn)
                {
                    if (insn.Insn is IJumpInsn jmp)
                    {
                        if (next() is InsnDef d && d.Label is string label && insn.Args[jmp.JumpArgIndex].Equals(new CGLabel<uint>(label))) remove();
                    }
                }
            }
        }

        public readonly record struct Settings(bool Simple, bool Destructive); // Default args don't work for some reason
    }
}
