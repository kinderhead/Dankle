using System;
using System.Text;

namespace Assembler
{
    public abstract class TokenParser
    {
        public abstract bool IsValid(StringBuilder soFar, char c);

        public virtual bool IsValidWhenFinished(StringBuilder soFar) => true;
    }

    public class ConstantToken(string text) : TokenParser
    {
        public readonly string Text = text;

        public override bool IsValid(StringBuilder soFar, char c) => soFar.Length < Text.Length && Text[soFar.Length] == c;
    }

    public class CollectiveOptionToken(char[] options) : TokenParser
    {
        public readonly char[] Options = options;

        public override bool IsValid(StringBuilder soFar, char c) => Options.Contains(c);
    }

    public class TextToken : TokenParser
    {
        public override bool IsValid(StringBuilder soFar, char c)
        {
            if (c == '$' || c == '_' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) return true;
            else if (soFar.Length != 0 && (c == '#' || (c >= '0' && c <= '9'))) return true;
            else return false;
        }
    }

    public class LabelToken : TokenParser
    {
        public override bool IsValid(StringBuilder soFar, char c)
        {
            if (soFar.Length != 0 && soFar[^1] == ':') return false;
            else if (c == '$' || c == '_' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) return true;
            else if (soFar.Length != 0 && (c == '#' || c == ':' || (c >= '0' && c <= '9'))) return true;
            else return false;
        }

        public override bool IsValidWhenFinished(StringBuilder soFar) => soFar.Length != 0 && soFar[^1] == ':';
    }

    public class IntegerToken : TokenParser
    {
        public override bool IsValid(StringBuilder soFar, char c)
        {
            c = char.ToLower(c);

            if (soFar.Length == 0 && c >= '0' && c <= '9') return true;
            else if (soFar.Length == 1 && (c == 'x' || c == 'b' || (c >= '0' && c <= '9'))) return true;
            else if (soFar.Length >= 2)
            {
                if (soFar[1] == 'x' && ((c >= 'a' && c <= 'f') || (c >= '0' && c <= '9'))) return true;
                else if (soFar[1] == 'b' && (c == '0' || c == '1')) return true;
                else if (c >= '0' && c <= '9') return true;
            }

            return false;
        }
    }

    public class RegisterToken : TokenParser
    {
        public override bool IsValid(StringBuilder soFar, char c)
        {
            if (soFar.Length == 0 && c == 'r') return true;
            else if (soFar.Length == 1 && c >= '0' && c <= '9') return true;
            else if (soFar.Length == 2 && soFar[1] == '1' && c >= '0' && c <= '5') return true;
            else return false;
        }
    }

    public class StringToken : TokenParser
    {
        public override bool IsValid(StringBuilder soFar, char c)
        {
            if (soFar.Length == 0 && c == '"') return true;
            else if (soFar.Length == 1 && soFar[^1] == '"') return true;
            else if (soFar.Length >= 2 && soFar[^1] != '"') return true;
            else if (soFar.Length >= 2 && soFar[^1] == '"' && soFar[^2] == '\\') return true;
            return false;
        }

        public override bool IsValidWhenFinished(StringBuilder soFar) => soFar.Length >= 2 && soFar[^1] == '"' && soFar[0] == '"';
    }

    public class CommentToken : TokenParser
    {
        public override bool IsValid(StringBuilder soFar, char c)
        {
            if (soFar.Length == 0 && c == ';') return true;
            else if (soFar.Length >= 1 && soFar[^1] != '\n') return true;
            return false;
        }
    }
}
