using Dankle;
using Newtonsoft.Json.Linq;
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
        public readonly Dictionary<string, uint> ExternalSymbols = [];
        public readonly Dictionary<string, uint> Symbols = [];
        public readonly List<Parser> Parsers = [];

        public byte[] AssembleAndLink(uint startAddr = 0, Computer? computer = null, ProgressBar? pb = null)
        {
            uint addr = startAddr;

            foreach (var i in Programs)
            {
                if (pb is not null) pb.Message = $"Assembling {Path.GetRelativePath(Environment.CurrentDirectory, i.Key)}...";

                var tokens = new Tokenizer(i.Value, false).Parse(pb);
                var parser = new Parser(tokens, addr, computer);

                addr += parser.SymbolPass();
                foreach (var e in parser.ExportedSymbols)
                {
                    Symbols[e] = parser.GetVariable<uint>(e);
                }

                Parsers.Add(parser);

                pb?.Tick();
            }

            var data = new byte[addr - startAddr];

            if (pb is not null) pb.Message = "Linking...";

            foreach (var i in Parsers)
            {
                foreach (var e in Parsers)
                {
                    if (i == e) continue;
                    i.ApplySymbols(e);
                }

                foreach (var e in ExternalSymbols)
                {
                    i.ImportableSymbols[e.Key] = e.Value;
                }

                i.Parse();
                i.GetBinary().CopyTo(data, i.StartAddr - startAddr);
            }

            pb?.Tick();

            // if (addr - startAddr >= 65536) throw new Exception("This may cause some issues");

            return data;
        }

        public void SaveSymbolfile(string path)
        {
            var data = new JObject();

            foreach (var i in Symbols)
            {
                if (i.Key == "_main") continue;
                data[i.Key] = i.Value;
            }

            File.WriteAllText(path, data.ToString());
        }

        public void LoadSymbolFile(string path)
        {
            var data = JObject.Parse(File.ReadAllText(path));

            foreach (var i in data)
            {
                ExternalSymbols[i.Key] = i.Value?.ToObject<uint>() ?? throw new InvalidOperationException($"Invalid JSON token {i.Value}");
            }
        }
	}
}
