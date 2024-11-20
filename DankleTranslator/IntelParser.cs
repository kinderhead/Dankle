using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Assembler;

namespace DankleTranslator
{
    public enum ArgumentType
    {
        Register,
        Integer,
        Label,
        Pointer
    }

    public readonly struct InsnSignature(string name, List<(ArgumentType, string)> args)
    {
        public readonly string Name = name;
        public readonly List<(ArgumentType, string)> Args = args;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is InsnSignature sig)
            {
                if (Name != sig.Name) return false;
                if (Args.Count != sig.Args.Count) return false;

                for (int i = 0; i < Args.Count; i++)
                {
                    if (Args[i].Item1 != sig.Args[i].Item1) return false;
                    if (Args[i].Item2 != sig.Args[i].Item2) return false;
                }

                return true;
            }
            else return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            foreach (var i in Args)
            {
                hash.Add(i.Item1);
                hash.Add(i.Item2);
            }
            return hash.ToHashCode();
        }

        public bool IsValid(string name, List<ArgumentType> types)
        {
            if (Name != name) return false;
            if (types.Count != Args.Count) return false;

            for (int i = 0; i < Args.Count; i++)
            {
                if (Args[i].Item1 != types[i]) return false;
            }

            return true;
        }

        public string Compile(string fmt)
        {
            for (int i = 0; i < Args.Count; i++)
            {
                fmt = fmt.Replace($"@{i + 1}", Args[i].Item2);
            }
            return fmt;
        }

        public static bool operator ==(InsnSignature a, InsnSignature b) => a.Equals(b);
        public static bool operator !=(InsnSignature a, InsnSignature b) => !a.Equals(b);
    }

    public class IntelParser(List<Token> tokens) : BaseParser<Token, Token.Type>(tokens)
    {
        public readonly List<string> PublicSymbols = [];

        public string Output { get; private set; } = "";

        private bool startedAssembly = false;

        public override void Parse()
        {
            while (Tokens.Count > 0)
            {
                var token = Tokens.Dequeue();

                if (token.Symbol == Token.Type.Public)
                {
                    if (TryGetToken(Token.Type.Text, out var sym)) PublicSymbols.Add(sym.Text);
                }
                else if (token.Symbol == Token.Type.Text && token.Text.EndsWith("_TEXT"))
                {
                    startedAssembly = true;
                }
                else if (token.Symbol == Token.Type.Label)
                {
                    Output += $"{token.Text}\n";
                }
                else if (token.Symbol == Token.Type.Text)
                {
                    ParseInsn(token);
                }
                else if (token.Symbol != Token.Type.Comma)
                {
                    throw new InvalidTokenException<Token, Token.Type>(token);
                }
            }
        }

        private void ParseInsn(Token token)
        {
            var sig = GetNextInsn(token);

            if (sig.IsValid("add", [ArgumentType.Register, ArgumentType.Integer])) Output += sig.Compile("\tld r12, @2\n\tadd @1, @1, r12");
            else throw new Exception("Invalid insn signature");

            Output += "\n";
        }

        private InsnSignature GetNextInsn(Token first)
        {
            Assume(first, Token.Type.Text);
            var name = first.Text;

            var next = Tokens.Peek();
            if (next.Line != first.Line) return new(name, []);

            List<(ArgumentType, string)> args = [];
            while (true)
            {
                args.Add(ParseArg());
                if (Tokens.Peek().Symbol == Token.Type.Comma)
                {
                    GetNextToken(Token.Type.Comma);
                }
                else break;
            }
            return new(name, args);
        }

        private (ArgumentType, string) ParseArg(Token? token = null)
        {
            var tok = token ?? Tokens.Dequeue();

            if (tok.Symbol == Token.Type.Register) return (ArgumentType.Register, $"{MapRegister(tok.Text)}");
            else if (tok.Symbol == Token.Type.Integer) return (ArgumentType.Integer, tok.Text);
            else if (tok.Symbol == Token.Type.Text) return (ArgumentType.Label, tok.Text);
            else Err(tok);
            return (ArgumentType.Label, "");
        }

        private static string MapRegister(string reg)
        {
            if (reg == "ax") return "r0";
            if (reg == "bx") return "r1";
            if (reg == "cx") return "r2";
            if (reg == "dx") return "r3";

            throw new Exception($"Unmapped register {reg}");
        }

        private void Err(Token tk)
        {
            throw new InvalidTokenException<Token, Token.Type>(tk);
        }
    }
}
