using System;

namespace DankleC.ASTObjects
{
    public enum PointerType
    {
        None,
        Pointer,
        ConstPointer
    }

    public class TypeSpecifier : IASTObject
    {
        public PointerType PointerType = PointerType.None;
        public bool IsConst = false;
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
    }

	public class UserTypeSpecifier(string type) : TypeSpecifier
	{
		public readonly string Type = type;
	}
}
