using System;
using System.Text;
using System.Text.RegularExpressions;
using ShellProgressBar;

namespace Assembler
{
    public abstract class BaseTokenizer<TToken, TType> where TType: struct, Enum where TToken : IToken<TType>
    {
        public string Input { get; protected set; }

        protected readonly OrderedDictionary<TType, TokenParser> TokenMap = [];
        private int Index;

        public BaseTokenizer(string input)
        {
            Input = input;
            GenerateTokenMap();
        }

		public List<TToken> Parse(ProgressBar? pb = null)
		{
			var tokens = new List<TToken>();

			var child = pb?.Spawn(Input.Length, "Tokenizing...");

			while (Index < Input.Length)
			{
				var loc = GetLineAndColumn(Index);

				List<KeyValuePair<TType, TokenParser>> possibilities = [.. TokenMap];
				Dictionary<TType, string> parsed = [];

				var i = Index;
				var soFar = new StringBuilder();
				while (possibilities.Count != 0)
				{
					for (var idex = 0; idex < possibilities.Count; idex++)
					{
						if (i >= Input.Length || !possibilities[idex].Value.IsValid(soFar, Input[i]))
						{
							if (possibilities[idex].Value.IsValidWhenFinished(soFar)) parsed[possibilities[idex].Key] = soFar.ToString();
							possibilities.RemoveAt(idex);
							idex--;
						}
					}

					if (possibilities.Count != 0)
					{
						soFar.Append(Input[i]);
						i++;
					}
				}

				var biggest = parsed.MaxBy(e => e.Value.Length);
				if (biggest.Value.Length == 0) throw new Exception($"Invalid symbol at {loc}: {Input[Index]}");

				var tk = MakeToken(biggest.Key, Index, biggest.Value, loc.Item1, loc.Item2);

				if (KeepToken(tk)) tokens.Add(tk);
				Index += tk.Text.Length;

				child?.Tick(Index);
			}

			child?.Dispose();

			return tokens;
		}

        public (int, int) GetLineAndColumn(int index)
		{
			var st = Input[..index];
			var line = st.Count(i => i == '\n');
			var col = st.Length - st.LastIndexOf('\n') - 1;
			return (line, col);
		}

        public abstract TToken MakeToken(TType symbol, int index, string text, int line, int column);

        protected virtual bool KeepToken(TToken tk) => true;
        protected abstract void GenerateTokenMap();
    }
}
