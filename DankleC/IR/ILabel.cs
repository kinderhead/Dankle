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
}
