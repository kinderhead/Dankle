using Dankle;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class Linker(IEnumerable<KeyValuePair<string, string>> programs)
    {
        public readonly Dictionary<string, string> Programs = new(programs);
        public readonly Dictionary<string, uint> Symbols = [];
        public readonly List<Parser> Parsers = [];

		public byte[] AssembleAndLink(uint startAddr = 0, Computer? computer = null, ProgressBar? pb = null)
        {
            uint addr = startAddr;

            foreach (var i in Programs)
            {
                if (pb is not null) pb.Message = $"Assembling {Path.GetRelativePath(Environment.CurrentDirectory, i.Key)}...";

                var tokens = new Tokenizer(i.Value, false).Parse();
                var parser = new Parser(tokens, addr, computer);

                addr += parser.SymbolPass();
                foreach (var e in parser.ExportedSymbols)
                {
                    Symbols[e] = parser.GetVariable<uint>(e);
                }

                Parsers.Add(parser);

                pb?.Tick();
            }

			var data = new byte[addr];

            if (pb is not null) pb.Message = "Linking...";

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

            pb?.Tick();

            // if (addr - startAddr > 65536) throw new Exception("This may cause some issues");

            return data;
		}
	}
}
