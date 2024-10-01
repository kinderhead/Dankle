using Dankle;
using Dankle.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleUI
{
	public class BufferTerminal(Computer computer, uint addr) : Terminal(computer, addr)
	{
		public string Buffer = "";

		public override void WriteOut(string text)
		{
			Interlocked.Exchange(ref Buffer, Buffer + text);
		}
	}
}
