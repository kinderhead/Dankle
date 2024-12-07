using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DankleC.ASTObjects;

namespace DankleC
{
    public class DankleCVisitor : CBaseVisitor<IASTObject>
    {
        public override IASTObject VisitRoot([NotNull] CParser.RootContext context)
        {
            var program = new ProgramNode();

            foreach (var i in context.children)
            {
                var def = Visit(i);
                if (def is FunctionNode func) program.Functions.Add(func);
            }

            return program;
        }

        public override IASTObject VisitFunction([NotNull] CParser.FunctionContext context)
        {
            var type = Visit(context.type());
            var name = context.Identifier().GetText();
			var scope = Visit(context.scope());

            return new FunctionNode(name, type, scope);
        }

		public override IASTObject VisitScope([NotNull] CParser.ScopeContext context)
		{
			var scope = new ScopeNode();

			foreach (var i in context.statement())
			{
				scope.Statements.Add((IStatement)Visit(i));
			}

			return scope;
		}

		public override IASTObject VisitConstantExpression([NotNull] CParser.ConstantExpressionContext context)
		{
			if (context.Constant() is ITerminalNode c)
			{
				var text = c.GetText();
				if (!text.Contains('.')) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), int.Parse(text));
				else throw new NotImplementedException();
			}
			else
			{
				var text = context.StringLiteral().GetText().Trim('"');
				var type = new BuiltinTypeSpecifier(BuiltinType.UnsignedChar)
				{
					IsConst = true,
					PointerType = PointerType.Pointer
				};
				return new ConstantExpression(type, text);
			}
		}

		#region Statements

		public override IASTObject VisitReturnStatement([NotNull] CParser.ReturnStatementContext context) => new ReturnStatement(Visit(context.expression()));

		#endregion

		#region Types

		public override IASTObject VisitType([NotNull] CParser.TypeContext context)
		{
			TypeSpecifier t;
			if (context.builtinType() is CParser.BuiltinTypeContext b) t = (TypeSpecifier)Visit(b);
			else t = (TypeSpecifier)Visit(context.userType());

			t.IsConst = context.@const is not null;
			t.PointerType = context.Star() is not null ? PointerType.Pointer : PointerType.None;
			if (context.pconst is not null && t.PointerType == PointerType.Pointer) t.PointerType = PointerType.ConstPointer;
			else t.IsConst = true;

			return t;
		}

		public override IASTObject VisitUnsignedChar([NotNull] CParser.UnsignedCharContext context) => new BuiltinTypeSpecifier(BuiltinType.UnsignedChar);
        public override IASTObject VisitSignedChar([NotNull] CParser.SignedCharContext context) => new BuiltinTypeSpecifier(BuiltinType.SignedChar);
		public override IASTObject VisitUnsignedShort([NotNull] CParser.UnsignedShortContext context) => new BuiltinTypeSpecifier(BuiltinType.UnsignedShort);
		public override IASTObject VisitSignedShort([NotNull] CParser.SignedShortContext context) => new BuiltinTypeSpecifier(BuiltinType.SignedShort);
		public override IASTObject VisitUnsignedInt([NotNull] CParser.UnsignedIntContext context) => new BuiltinTypeSpecifier(BuiltinType.UnsignedInt);
		public override IASTObject VisitSignedInt([NotNull] CParser.SignedIntContext context) => new BuiltinTypeSpecifier(BuiltinType.SignedInt);
		public override IASTObject VisitUnsignedLong([NotNull] CParser.UnsignedLongContext context) => new BuiltinTypeSpecifier(BuiltinType.UnsignedLong);
		public override IASTObject VisitSignedLong([NotNull] CParser.SignedLongContext context) => new BuiltinTypeSpecifier(BuiltinType.SignedLong);
		public override IASTObject VisitUnsignedLongLong([NotNull] CParser.UnsignedLongLongContext context) => new BuiltinTypeSpecifier(BuiltinType.UnsignedLongLong);
		public override IASTObject VisitSignedLongLong([NotNull] CParser.SignedLongLongContext context) => new BuiltinTypeSpecifier(BuiltinType.SignedLongLong);
		public override IASTObject VisitFloat([NotNull] CParser.FloatContext context) => new BuiltinTypeSpecifier(BuiltinType.Float);
		public override IASTObject VisitDouble([NotNull] CParser.DoubleContext context) => new BuiltinTypeSpecifier(BuiltinType.Double);
		public override IASTObject VisitLongDouble([NotNull] CParser.LongDoubleContext context) => new BuiltinTypeSpecifier(BuiltinType.LongDouble);

		public override IASTObject VisitBuiltinType([NotNull] CParser.BuiltinTypeContext context)
		{
            if (context.integerType() is CParser.IntegerTypeContext i) return Visit(i);
            else if (context.Void() is not null) return new BuiltinTypeSpecifier(BuiltinType.Void);
            else return new BuiltinTypeSpecifier(BuiltinType.Bool);
		}

		public override IASTObject VisitUserType([NotNull] CParser.UserTypeContext context)
		{
            return new UserTypeSpecifier(context.Identifier().GetText());
		}

		#endregion

		#region Visit Overloads

		public TypeSpecifier Visit(CParser.TypeContext context) => (TypeSpecifier)Visit((IParseTree)context);
		public ScopeNode Visit(CParser.ScopeContext context) => (ScopeNode)Visit((IParseTree)context);
		public IExpression Visit(CParser.ExpressionContext context) => (IExpression)Visit((IParseTree)context);

		#endregion
	}
}
