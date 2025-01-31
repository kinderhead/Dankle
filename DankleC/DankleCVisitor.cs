using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DankleC.ASTObjects;
using DankleC.ASTObjects.Expressions;

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
				scope.Statements.Add((Statement)Visit(i));
			}

			return scope;
		}

		#region Expressions

		public override IASTObject VisitConstantExpression([NotNull] CParser.ConstantExpressionContext context)
		{
			if (context.Constant() is ITerminalNode c)
			{
				var text = c.GetText();
				if (!text.Contains('.'))
				{
					var num = long.Parse(text);
					if (num >= sbyte.MinValue && num <= sbyte.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedChar), (sbyte)num);
					else if (num >= byte.MinValue && num <= byte.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), (byte)num);
					else if (num >= short.MinValue && num <= short.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedShort), (short)num);
					else if (num >= ushort.MinValue && num <= ushort.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedShort), (ushort)num);
					else if (num >= int.MinValue && num <= int.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), (int)num);
					else if (num >= uint.MinValue && num <= uint.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedInt), (uint)num);
					else throw new NotImplementedException();
				}
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

		public override IASTObject VisitVariableExpression([NotNull] CParser.VariableExpressionContext context) => new VariableExpression(context.Identifier().GetText());

		public override IASTObject VisitPrimaryExpression([NotNull] CParser.PrimaryExpressionContext context)
		{
			if (context.expression() is CParser.ExpressionContext ex) return Visit(ex);
			else return Visit(context.children[0]);
		}

		public override IASTObject VisitAssignmentExpression([NotNull] CParser.AssignmentExpressionContext context)
		{
			if (context.additiveExpression() is CParser.AdditiveExpressionContext add) return Visit(add);
			return new AssignmentExpression(context.Identifier().GetText(), (IExpression)Visit(context.assignmentExpression()));
		}

		public override IASTObject VisitAdditiveExpression([NotNull] CParser.AdditiveExpressionContext context)
		{
			if (context.additiveExpression() is null) return Visit(context.multiplicativeExpression());
			return new ArithmeticExpression((IExpression)Visit(context.multiplicativeExpression()), ArithmeticOperation.Addition, (IExpression)Visit(context.additiveExpression()));
		}

		public override IASTObject VisitMultiplicativeExpression([NotNull] CParser.MultiplicativeExpressionContext context)
		{
			if (context.multiplicativeExpression() is null) return Visit(context.unaryExpression());
			return new ArithmeticExpression((IExpression)Visit(context.unaryExpression()), ArithmeticOperation.Multiplication, (IExpression)Visit(context.multiplicativeExpression()));
		}

		#endregion

		#region Statements

		public override IASTObject VisitReturnStatement([NotNull] CParser.ReturnStatementContext context) => new ReturnStatement(Visit(context.expression()));
		public override IASTObject VisitAssignmentStatement([NotNull] CParser.AssignmentStatementContext context) => new InitAssignmentStatement(Visit(context.type()), context.Identifier().GetText(), Visit(context.expression()));

		public override IASTObject VisitStatement([NotNull] CParser.StatementContext context) => Visit(context.children[0]);

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
			else if (context.pconst is not null) t.IsConst = true;

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
