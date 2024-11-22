using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DankleTranslator
{
	public enum ArgumentType
	{
		Register,
		Integer,
		Label,
		Pointer,
		BytePointer,

		SS,
		CS
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
			foreach (var i in Macros)
			{
				fmt = fmt.Replace($"%{i.Key}", i.Value);
			}

			for (int i = 0; i < Args.Count; i++)
			{
				if (Args[i].Item1 == ArgumentType.Pointer || Args[i].Item1 == ArgumentType.BytePointer)
				{
					string offset;
					if (Args[i].Item2.Contains(',')) offset = Args[i].Item2.Split(',')[0];
					else if (Args[i].Item2.Contains('-')) offset = GetIndirectHighReg(Args[i].Item2.Split('-')[0]);
					else if (Args[i].Item2.Contains('+')) offset = GetIndirectHighReg(Args[i].Item2.Split('+')[0]);
					else offset = GetIndirectHighReg(Args[i].Item2);

					fmt = fmt.Replace($"@ptr{i + 1}", $"[{offset},{Args[i].Item2.Split(',').Last()}]");
				}

				fmt = fmt.Replace($"@{i + 1}", Args[i].Item2);
			}

			foreach (var i in Macros)
			{
				fmt = fmt.Replace($"%{i.Key}", i.Value);
			}
			
			return fmt;
		}

		public static bool operator ==(InsnSignature a, InsnSignature b) => a.Equals(b);
		public static bool operator !=(InsnSignature a, InsnSignature b) => !a.Equals(b);

		public static readonly Dictionary<string, string> Macros = [];

		public static string GetIndirectHighReg(string reg)
		{
			reg = reg.Split("@")[0];
			if (reg == "r1") return "r4";
			else if (reg == "r6") return "r12";
			else if (reg == "r5") return "r4";
			else if (reg == "r7") return "r4";
			else throw new Exception($"Cannot address register {reg}");
		}

		static InsnSignature()
		{
			Macros["tmpalt"] = "r10";
			Macros["tmp"] = "r11";
			Macros["trash"] = "r9";
			Macros["es"] = "r8";
			Macros["ds"] = "r4";
			Macros["ldtmp2"] = "\tld r11, @2\n";
			Macros["h"] = "";
			Macros["l"] = "";
		}
	}
}
