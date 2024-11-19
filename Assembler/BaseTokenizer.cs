using System;
using System.Text.RegularExpressions;

namespace Assembler
{
    public abstract class BaseTokenizer<TToken, TType> where TType: struct, Enum where TToken : IToken<TType>
    {
        public string Input { get; protected set; }

        protected readonly OrderedDictionary<TType, Regex> TokenMap = [];
        private int Index;

        public BaseTokenizer(string input)
        {
            Input = input;
            GenerateTokenMap();
        }

		public List<TToken> Parse()
		{
			var tokens = new List<TToken>();

			while (Index < Input.Length)
			{
				var possibleTokens = new List<TToken>();
				foreach (var i in TokenMap)
				{
					var match = i.Value.Match(Input[Index..]);
					if (match.Success)
					{
						var loc = GetLineAndColumn(Index);
						possibleTokens.Add(MakeToken(i.Key, Index, match.Value, loc.Item1, loc.Item2));
					}
				}

				if (possibleTokens.Count == 0) throw new Exception($"Invalid symbol at {GetLineAndColumn(Index)}: {Input[Index]}");

				var tk = possibleTokens.MaxBy(i => i.Text.Length);
                if (tk is null) continue;

				if (KeepToken(tk)) tokens.Add(tk);
				Index += tk.Text.Length;
			}

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
