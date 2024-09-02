using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dankle.Components
{
	public class CPUCore(Computer computer) : Component(computer)
	{
		public override string Name => "CPUCore";

		private readonly ushort[] Registers = new ushort[16];

		private Memory? mainMemory;
		private Memory MainMemory { get => mainMemory ?? throw new Exception("Accessed memory before init"); set => mainMemory = value; }

		private ushort ProgramCounter => Registers[15];
		private ushort StackPointer => Registers[14];

		public byte ReadMem(uint addr) => MainMemory.Read(addr, this);
		public ushort ReadMem16(uint addr) => MainMemory.Read16(addr, this);
		public void WriteMem(uint addr, byte value) => MainMemory.Write(addr, value, this);

		protected override void Init()
		{
			MainMemory = Computer.GetComponent<Memory>();
		}

		protected override void Process()
		{
			while (!ShouldStop)
			{
				HandleMessage(false);
			}
		}
	}
}
