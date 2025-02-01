using Dankle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class Linker(List<string> programs)
    {
        public readonly List<string> Programs = programs;
        public readonly Dictionary<string, uint> Symbols = [];
        public readonly List<Parser> Parsers = [];

		public byte[] AssembleAndLink(uint startAddr = 0, Computer? computer = null)
        {
            uint addr = startAddr;
            foreach (var i in Programs)
            {
                var tokens = new Tokenizer(i, false).Parse();
                var parser = new Parser(tokens, addr, computer);

                addr += parser.SymbolPass();
				foreach (var e in parser.ExportedSymbols)
				{
                    Symbols[e] = parser.GetVariable<uint>(e);
				}

				Parsers.Add(parser);
            }

			var data = new byte[addr];

			foreach (var i in Parsers)
			{
				foreach (var e in Parsers)
				{
                    if (i == e) continue;
                    i.ApplySymbols(e);
				}

                i.Parse();
                i.GetBinary().CopyTo(data, i.StartAddr - startAddr);
			}

            // if (addr - startAddr > 65536) throw new Exception("This may cause some issues");

            return data;
		}
	}
}
