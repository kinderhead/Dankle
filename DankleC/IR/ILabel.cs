using System;

namespace DankleC.IR
{
    public interface ILabel
    {
        public string Resolve(CodeGen gen);
    }

    public class IRLabel(string name) : IRInsn, ILabel
    {
        public readonly string Name = name;

        public override void Compile(CodeGen gen)
        {
            Add(Name);
        }

        public string Resolve(CodeGen gen) => Name;
    }

    public class IRLogicLabel() : IRInsn, ILabel
    {
        public string? Name { get; private set; }

        public override void Compile(CodeGen gen)
        {
            Add(Resolve(gen));
        }

        public string Resolve(CodeGen gen)
        {
            Name ??= gen.GetLogicLabel();
            return Name;
        }
    }
    
    public class IRStaticVariableLabel(IRFunction func, string name, int num) : IRInsn, ILabel
	{
        public readonly IRFunction Function = func;
        public readonly string Name = name;
        public readonly int Index = num;
        public string Label => $"V${Function.Name}${Index}_{Name}";

		public override void Compile(CodeGen gen)
        {
            Add(Resolve(gen));
        }

        public string Resolve(CodeGen gen)
        {
            return Label;
        }
    }
}
