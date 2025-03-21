using System;
using System.Text;

namespace DankleC.ASTObjects
{
	public abstract class TypeSpecifier : IASTObject
	{
		public bool IsConst = false;
		public bool IsExtern = false;
		public bool IsStatic = false;

		public int Size { get => GetTypeSize(); }

		public abstract bool AreEqual(TypeSpecifier a);
		public abstract string GetName();
		public abstract bool IsNumber();
		public abstract bool IsSigned();
		public PointerTypeSpecifier AsPointer() => new(this);

		public static bool operator ==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
		public static bool operator !=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
		protected abstract int GetTypeSize();

		//public override string ToString()
		//{
		//          var builder = new StringBuilder();

		//          if (IsConst) builder.Append("const ");
		//          builder.Append(GetName());

		//          if (PointerType == PointerType.Pointer) builder.Append("* ");
		//          else if (PointerType == PointerType.ConstPointer) builder.Append("* const");

		//          return builder.ToString();
		//}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			if (obj is null) return false;
			if (obj is TypeSpecifier type)
			{
				if (type.IsConst != IsConst) return false;
				return AreEqual(type);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static TypeSpecifier GetBigger(TypeSpecifier a, TypeSpecifier b) => a.Size >= b.Size ? a : b;

		public static BuiltinTypeSpecifier GetGenericForSize(int size) => size switch
		{
			1 => new BuiltinTypeSpecifier(BuiltinType.UnsignedChar),
			2 => new BuiltinTypeSpecifier(BuiltinType.UnsignedShort),
			4 => new BuiltinTypeSpecifier(BuiltinType.UnsignedInt),
			_ => throw new InvalidOperationException(),
		};

		public static TypeSpecifier GetOperationType(TypeSpecifier left, TypeSpecifier right)
		{
			var largest = left.Size >= right.Size ? left : right;
			var smallest = left.Size >= right.Size ? right : left;

			if (left == right) return left;
			else if (largest.IsSigned() == smallest.IsSigned()) return largest;
			else if (largest.Size > smallest.Size) return largest;
			else
			{
				if (largest.IsSigned()) return smallest;
				else return largest;
			}
		}
	}

	public enum BuiltinType
	{
		UnsignedChar,
		SignedChar,
		UnsignedShort,
		SignedShort,
		UnsignedInt,
		SignedInt,
		UnsignedLong,
		SignedLong,
		UnsignedLongLong,
		SignedLongLong,
		Float,
		Double,
		LongDouble,

		Void,
		Bool
	}

	public class BuiltinTypeSpecifier(BuiltinType type) : TypeSpecifier
	{
		public readonly BuiltinType Type = type;

		public override bool AreEqual(TypeSpecifier a)
		{
			if (a is not BuiltinTypeSpecifier type || a.GetType().IsSubclassOf(typeof(BuiltinTypeSpecifier))) return false;
			return Type == type.Type;
		}

		public override string GetName() => Enum.GetName(Type) ?? "<err>";

		public override bool IsNumber() => Type != BuiltinType.Void;

		public override bool IsSigned()
		{
			return Type switch
			{
				BuiltinType.SignedChar or BuiltinType.SignedShort or BuiltinType.SignedInt or BuiltinType.SignedLong or BuiltinType.SignedLongLong or BuiltinType.Float or BuiltinType.Double or BuiltinType.LongDouble => true,
				_ => false,
			};
		}

		protected override int GetTypeSize()
		{
			return Type switch
			{
				BuiltinType.UnsignedChar or BuiltinType.SignedChar or BuiltinType.Bool => 1,
				BuiltinType.UnsignedShort or BuiltinType.SignedShort => 2,
				BuiltinType.Float or BuiltinType.UnsignedInt or BuiltinType.SignedInt => 4,
				BuiltinType.Double or BuiltinType.LongDouble or BuiltinType.UnsignedLong or BuiltinType.SignedLong => 8,
				BuiltinType.UnsignedLongLong or BuiltinType.SignedLongLong => 8,
				BuiltinType.Void => 0,
				_ => throw new NotImplementedException(),
			};
		}
	}

	public class UserTypeSpecifier(string type) : TypeSpecifier
	{
		public readonly string Type = type;

		public override bool AreEqual(TypeSpecifier a)
		{
			if (a is not UserTypeSpecifier type) return false;
			return Type == type.Type;
		}

		public override string GetName() => Type;

		public override bool IsNumber() => false;
		public override bool IsSigned() => false;

		protected override int GetTypeSize()
		{
			throw new NotImplementedException();
		}
	}

	public class PointerTypeSpecifier(TypeSpecifier inner) : BuiltinTypeSpecifier(BuiltinType.UnsignedInt)
	{
		public readonly TypeSpecifier Inner = inner;

		public override bool AreEqual(TypeSpecifier a)
		{
			if (a is not PointerTypeSpecifier type) return false;
			return Inner == type.Inner;
		}

		public override string GetName() => Inner.GetName() + "*";
	}

	public class ArrayTypeSpecifier(TypeSpecifier inner, int size) : TypeSpecifier()
	{
		public readonly TypeSpecifier Inner = inner;
		public readonly int ArraySize = size;

		public override bool AreEqual(TypeSpecifier a)
		{
			if (a is not ArrayTypeSpecifier type) return false;
			return Inner == type.Inner;
		}

		public override string GetName() => $"{Inner.GetName()}[{ArraySize}]";

		public override bool IsNumber() => true;
		public override bool IsSigned() => false;
		protected override int GetTypeSize() => ArraySize * Inner.Size;
	}

	public class FunctionTypeSpecifier : TypeSpecifier
	{
		public readonly TypeSpecifier ReturnType;
		public readonly ParameterList Parameters;

		public FunctionTypeSpecifier(TypeSpecifier ret, ParameterList parameters) : base()
		{
			if (ret is ArrayTypeSpecifier) throw new InvalidOperationException("Array type cannot be function return type");
			ReturnType = ret;
			Parameters = parameters;
		}

		public override bool AreEqual(TypeSpecifier a)
		{
			if (a is not FunctionTypeSpecifier type) return false;
			return ReturnType == type.ReturnType;
		}

		public override string GetName() => $"{ReturnType.GetName()} (*)()";
		public override bool IsNumber() => false;
		public override bool IsSigned() => false;

		protected override int GetTypeSize() => 4;
	}

    public class StructTypeSpecifier(string name, List<DeclaratorPair> props) : TypeSpecifier
    {
		public readonly string Name = name;
		public readonly List<DeclaratorPair> Members = props;

		public TypeSpecifier GetMember(string name)
		{
			foreach (var i in Members)
			{
				if (i.Name == name) return i.Type;
			}

			throw new InvalidOperationException($"Member \"{Name}\" is not in {GetName()}");
		}

		public int GetOffset(string name)
		{
			int offset = 0;

			foreach (var i in Members)
			{
				if (i.Name == name) break;
				offset += i.Type.Size;
			}

			if (offset == Size) throw new InvalidOperationException($"Member \"{Name}\" is not in {GetName()}");

			return offset;
		}

		public override bool AreEqual(TypeSpecifier a)
		{
			if (a is not StructTypeSpecifier type || Name != type.Name || Members.Count != type.Members.Count) return false;

			for (int i = 0; i < Members.Count; i++)
			{
				if (Members[i].Name != type.Members[i].Name || Members[i].Type != type.Members[i].Type) return false;
			}

			return true;
		}

		public override string GetName() => $"struct {Name}";
		public override bool IsNumber() => false;
		public override bool IsSigned() => false;

		protected override int GetTypeSize()
		{
			int size = 0;

			foreach (var i in Members)
			{
				size += i.Type.Size;
			}

			return size;
        }
    }
}
