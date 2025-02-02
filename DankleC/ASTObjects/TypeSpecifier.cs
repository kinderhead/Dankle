using System;
using System.Text;

namespace DankleC.ASTObjects
{
    public abstract class TypeSpecifier : IASTObject
    {
        public bool IsConst = false;

        public int Size { get => GetTypeSize(); }

        public abstract bool AreEqual(TypeSpecifier a);
        public abstract string GetName();
        public abstract bool IsNumber();
        public abstract bool IsSigned();
        public PointerTypeSpecifier AsPointer() => new(this);

        public static bool operator==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
        public static bool operator!=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
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
        Bool,

        String
    }

    public class BuiltinTypeSpecifier(BuiltinType type) : TypeSpecifier
    {
        public readonly BuiltinType Type = type;

		public override bool AreEqual(TypeSpecifier a)
		{
            if (a is not BuiltinTypeSpecifier type) return false;
            return Type == type.Type;
		}

		public override string GetName() => Enum.GetName(Type) ?? "<err>";

        public override bool IsNumber() => Type != BuiltinType.Void && Type != BuiltinType.Bool && Type != BuiltinType.String;

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
}
