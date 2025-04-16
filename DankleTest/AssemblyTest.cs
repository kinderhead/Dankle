using Assembler;
using Dankle;
using Dankle.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleTest
{
	[TestClass]
	public class AssemblyTest
	{
		public static Computer RunProgram(string prog)
		{
			var computer = new Computer(0xF0000u);
			computer.AddComponent<Terminal>(0xFFFFFFF0u);

			var linker = new Linker([new("prog", prog)]);
			computer.WriteMem(0x10000u, linker.AssembleAndLink(0x10000u, computer));
			computer.GetComponent<CPUCore>().ProgramCounter = linker.Symbols["main"];

			computer.Run();
			return computer;
		}

		[TestMethod]
		public void TestUDIVL()
		{
			var core = RunProgram(@"
export main
main:
	udivl 0x12345678, 4, (r0, r1)
	hlt
").MainCore;
			Assert.AreEqual(0x48D, core.Registers[0]);
			Assert.AreEqual(0x159E, core.Registers[1]);
		}
	}
}
