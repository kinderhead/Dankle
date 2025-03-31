using System;
using System.Text;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Assembler;
using DankleC.ASTObjects;
using DankleC.ASTObjects.Expressions;

namespace DankleC
{
	public class DankleCVisitor : CBaseVisitor<IASTObject>
	{
		// Storing structs and typedefs here makes my life easier and also helps me to follow the C standard :)
		public readonly Dictionary<string, StructTypeSpecifier> Structs = [];
		public readonly Dictionary<string, TypeSpecifier> UserTypes = [];

		public override IASTObject VisitRoot([NotNull] CParser.RootContext context)
		{
			var program = new ProgramNode();

			foreach (var i in context.children)
			{
				var def = Visit(i);
				if (def is FunctionNode func) program.Functions.Add(func);
				else if (def is ScopeNode decls)
				{
					foreach (var decl in decls.Statements)
					{
						if (decl is DeclareStatement d)
						{
							if (d.Type.Innermost.IsExtern || d.Type is FunctionTypeSpecifier)
							{
								d.Type.IsExtern = true;
								program.Externs[d.Name] = d.Type;
							}
							else if (d.Type.Innermost.IsTypedef)
							{
								UserTypes[d.Name] = d.Type;
							}
						}
						else throw new NotImplementedException();
					}
				}
			}

			return program;
		}

		public override IASTObject VisitFunction([NotNull] CParser.FunctionContext context)
		{
			var pair = Visit(context.declarator(), Visit(context.declarationSpecifier()));
			var type = (FunctionTypeSpecifier)pair.Type;
			type.IsStatic = context.Static() is not null;
			var scope = Visit(context.scope());

			return new FunctionNode(pair.Name, type, scope);
		}

		public override IASTObject VisitScope([NotNull] CParser.ScopeContext context)
		{
			var scope = new ScopeNode();

			foreach (var i in context.statement())
			{
				scope.Statements.Add(Visit(i));
			}

			return scope;
		}

		public override IASTObject VisitParameterList([NotNull] CParser.ParameterListContext context)
		{
			var p = new ParameterList([], context.Ellipsis() is not null);

			foreach (var i in context.parameterDeclaration())
			{
				if (i.declarator() is not null) p.Parameters.Add(Visit(i.declarator(), Visit(i.declarationSpecifier())));
				else p.Parameters.Add(new(Visit(i.abstractDeclarator(), Visit(i.declarationSpecifier())), ""));
			}

			return p;
		}

		public override IASTObject VisitArgumentList([NotNull] CParser.ArgumentListContext context)
		{
			var args = new ArgumentList([]);

			for (int i = 0; i < context.expression().Length; i++)
			{
				args.Arguments.Add(Visit(context.expression()[i]));
			}

			return args;
		}

        public override IASTObject VisitErrorNode(IErrorNode node)
        {
			throw new Exception($"Invalid symbol \"{node.GetText()}\" at {node.Symbol.Line}:{node.Symbol.Column}");
        }

		#region Expressions

		public static ConstantExpression GetSmallestConstantExpression(Int128 num, bool isIntMin = true)
		{
			if (!isIntMin && num >= sbyte.MinValue && num <= sbyte.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedChar), (sbyte)num);
			else if (!isIntMin && num >= byte.MinValue && num <= byte.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), (byte)num);
			else if (!isIntMin && num >= short.MinValue && num <= short.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedShort), (short)num);
			else if (!isIntMin && num >= ushort.MinValue && num <= ushort.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedShort), (ushort)num);
			else if (num >= int.MinValue && num <= int.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedInt), (int)num);
			else if (num >= uint.MinValue && num <= uint.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedInt), (uint)num);
			else if (num >= long.MinValue && num <= long.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedLong), (long)num);
			else if (num >= ulong.MinValue && num <= ulong.MaxValue) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedLong), (ulong)num);
			else throw new NotImplementedException();
		}

		public override IASTObject VisitExpression([NotNull] CParser.ExpressionContext context)
		{
			return Visit(context.children[0]);
            // IExpression expr = (IExpression)Visit(context.assignmentExpression()[0]);
			// for (int i = 0; i < context.children.Count; i++)
			// {
			// 	if (i == 0) continue;
			// 	expr = new CommaExpression(expr, (IExpression)Visit(context.children[++i]));
			// }
			// return expr;
		}

        public override IASTObject VisitConstantExpression([NotNull] CParser.ConstantExpressionContext context)
		{
			if (context.Constant() is ITerminalNode c) return VisitInt(c);
			else
			{
				return new StringLiteralExpression(context.StringLiteral().GetText().Trim('"'), new PointerTypeSpecifier(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar) { IsConst = true }));
			}
		}

		public override IASTObject VisitVariableExpression([NotNull] CParser.VariableExpressionContext context) => new VariableExpression(context.Identifier().GetText());

		public override IASTObject VisitUnaryExpression([NotNull] CParser.UnaryExpressionContext context)
		{
			if (context.postfixExpression() is CParser.PostfixExpressionContext pe) return Visit(pe);
			else if (context.Star() is not null) return new DerefExpression((IExpression)Visit(context.castExpression()));
			else if (context.PlusPlus() is not null) return new PreIncrementExpression((UnresolvedLValue)Visit(context.unaryExpression()));
			else if (context.MinusMinus() is not null) return new PreDecrementExpression((UnresolvedLValue)Visit(context.unaryExpression()));
			else if (context.And() is not null) return new RefExpression((UnresolvedLValue)Visit(context.castExpression()));
			else if (context.Minus() is not null) return new ArithmeticExpression(new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedChar), 0), ArithmeticOperation.Subtraction, (IExpression)Visit(context.castExpression()));
			else if (context.Not() is not null) return new NotExpression((IExpression)Visit(context.castExpression()));
			else if (context.Tilde() is not null) return new BitwiseNotExpression((IExpression)Visit(context.castExpression()));
			else if (context.type() is not null) return GetSmallestConstantExpression(Visit(context.type()).Size);
			else if (context.Sizeof() is not null && context.unaryExpression() is not null) return new SizeofExpression((IExpression)Visit(context.unaryExpression()));
			else throw new NotImplementedException();
		}

		public override IASTObject VisitCastExpression([NotNull] CParser.CastExpressionContext context)
		{
			if (context.unaryExpression() is CParser.UnaryExpressionContext pe) return Visit(pe);
			else return new UnresolvedCastExpression((IExpression)Visit(context.castExpression()), Visit(context.type()));
		}

		public override IASTObject VisitPostfixExpression([NotNull] CParser.PostfixExpressionContext context)
		{
			var expr = (IExpression)Visit(context.primaryExpression());

			foreach (var i in context.partialPostfixExpression())
			{
				if (i.PlusPlus() is not null) expr = new PostIncrementExpression((UnresolvedLValue)expr);
				else if (i.MinusMinus() is not null) expr = new PostDecrementExpression((UnresolvedLValue)expr);
				else if (i.LeftParen() is not null) expr = new CallExpression(expr, i.argumentList() is not null ? (ArgumentList)Visit(i.argumentList()) : new([]));
				else if (i.Dot() is not null) expr = new MemberExpression(expr, i.Identifier().GetText());
				else if (i.Arrow() is not null) expr = new PointerMemberExpression(expr, i.Identifier().GetText());
				else if (i.LeftBracket() is not null) expr = new IndexExpression(expr, Visit(i.expression()));
				else throw new NotImplementedException();
			}

			return expr;
		}

		public override IASTObject VisitPrimaryExpression([NotNull] CParser.PrimaryExpressionContext context)
		{
			if (context.expression() is CParser.ExpressionContext ex) return Visit(ex);
			else return Visit(context.children[0]);
		}

		public override IASTObject VisitAssignmentExpression([NotNull] CParser.AssignmentExpressionContext context)
		{
			if (context.conditionalExpression() is CParser.ConditionalExpressionContext add) return Visit(add);

			var op = context.children[1].GetText() switch
			{
				"=" => ArithmeticOperation.Null,
				"+=" => ArithmeticOperation.Addition,
				"-=" => ArithmeticOperation.Subtraction,
				"*=" => ArithmeticOperation.Multiplication,
				"/=" => ArithmeticOperation.Division,
				"%=" => ArithmeticOperation.Modulo,
				"<<=" => ArithmeticOperation.LeftShift,
				">>=" => ArithmeticOperation.RightShift,
				"|=" => ArithmeticOperation.InclusiveOr,
				"^=" => ArithmeticOperation.ExclusiveOr,
				"&=" => ArithmeticOperation.And,
				_ => throw new NotImplementedException()
			};

			return new AssignmentExpression((UnresolvedLValue)Visit(context.unaryExpression()), op, (IExpression)Visit(context.assignmentExpression()));
		}

		public override IASTObject VisitConditionalExpression([NotNull] CParser.ConditionalExpressionContext context)
		{
			if (context.Question() is not null) return new ConditionalExpression((IExpression)Visit(context.logicalOrExpression()), Visit(context.expression()), (IExpression)Visit(context.conditionalExpression()));
			return Visit(context.logicalOrExpression());
		}

        public override IASTObject VisitEqualityExpression([NotNull] CParser.EqualityExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.relationalExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;

				var op = context.children[i].GetText() == "==" ? EqualityOperation.Equals : EqualityOperation.NotEquals;
				expr = new EqualityExpression(expr, op, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

		public override IASTObject VisitRelationalExpression([NotNull] CParser.RelationalExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.shiftExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;

				var op = context.children[i].GetText() switch
				{
					"<" => EqualityOperation.LessThan,
					"<=" => EqualityOperation.LessThanOrEqual,
					">" => EqualityOperation.GreaterThan,
					">=" => EqualityOperation.GreaterThanOrEqual,
					_ => throw new NotImplementedException()
				};
				expr = new EqualityExpression(expr, op, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

        public override IASTObject VisitShiftExpression([NotNull] CParser.ShiftExpressionContext context)
        {
            IExpression expr = (IExpression)Visit(context.additiveExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;

				var op = context.children[i].GetText() == ">>" ? ArithmeticOperation.RightShift : ArithmeticOperation.LeftShift;
				expr = new ArithmeticExpression(expr, op, (IExpression)Visit(context.children[++i]));
			}
			return expr;
        }

		public override IASTObject VisitAdditiveExpression([NotNull] CParser.AdditiveExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.multiplicativeExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;

				var op = context.children[i].GetText() == "+" ? ArithmeticOperation.Addition : ArithmeticOperation.Subtraction;
				expr = new ArithmeticExpression(expr, op, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

		public override IASTObject VisitMultiplicativeExpression([NotNull] CParser.MultiplicativeExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.castExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;

				var op = context.children[i].GetText() switch
				{
					"*" => ArithmeticOperation.Multiplication,
					"/" => ArithmeticOperation.Division,
					"%" => ArithmeticOperation.Modulo,
					_ => throw new InvalidOperationException()
				};
				expr = new ArithmeticExpression(expr, op, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

		public override IASTObject VisitLogicalOrExpression([NotNull] CParser.LogicalOrExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.logicalAndExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;
				expr = new LogicalExpression(expr, LogicalOperation.Or, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

		public override IASTObject VisitLogicalAndExpression([NotNull] CParser.LogicalAndExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.inclusiveOrExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;
				expr = new LogicalExpression(expr, LogicalOperation.And, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

		public override IASTObject VisitInclusiveOrExpression([NotNull] CParser.InclusiveOrExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.exclusiveOrExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;
				expr = new ArithmeticExpression(expr, ArithmeticOperation.InclusiveOr, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

		public override IASTObject VisitExclusiveOrExpression([NotNull] CParser.ExclusiveOrExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.andExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;
				expr = new ArithmeticExpression(expr, ArithmeticOperation.ExclusiveOr, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

		public override IASTObject VisitAndExpression([NotNull] CParser.AndExpressionContext context)
		{
			IExpression expr = (IExpression)Visit(context.equalityExpression()[0]);
			for (int i = 0; i < context.children.Count; i++)
			{
				if (i == 0) continue;
				expr = new ArithmeticExpression(expr, ArithmeticOperation.And, (IExpression)Visit(context.children[++i]));
			}
			return expr;
		}

		public override IASTObject VisitLvalue([NotNull] CParser.LvalueContext context) => Visit(context.children[0]);

		#endregion

		#region Statements

		public override IASTObject VisitReturnStatement([NotNull] CParser.ReturnStatementContext context) => new ReturnStatement(context.expression() is not null ? Visit(context.expression()) : null);

		public override IASTObject VisitDeclaration([NotNull] CParser.DeclarationContext context)
		{
			var baseType = Visit(context.declarationSpecifier());

			var decls = new ScopeNode(false);
			foreach (var i in context.initDeclarator())
			{
				var type = Visit(i.declarator(), baseType);
				type.Type.IsStatic = context.Static() is not null;
				if (i.expression() is CParser.ExpressionContext expr) decls.Statements.Add(new InitAssignmentStatement(type.Type, type.Name, Visit(expr)));
				else decls.Statements.Add(new DeclareStatement(type.Type, type.Name));
			}

			return decls;
		}

		public override IASTObject VisitExpressionStatement([NotNull] CParser.ExpressionStatementContext context) => new ExpressionStatement(Visit(context.expression()));
		public override IASTObject VisitIfStatement([NotNull] CParser.IfStatementContext context) => new IfStatement(Visit(context.expression()), Visit(context.statement()[0]), context.Else() is null ? null : Visit(context.statement()[1]));
		public override IASTObject VisitWhileStatement([NotNull] CParser.WhileStatementContext context) => new WhileStatement(Visit(context.expression()), Visit(context.statement()), context.Do() is not null);
		public override IASTObject VisitForStatement([NotNull] CParser.ForStatementContext context) => new ForStatement(context.stmt1 is not null ? (Statement)Visit(context.stmt1) : null, context.expression() is not null ? Visit(context.expression()) : null, context.stmt3 is not null ? (Statement)Visit(context.stmt3) : null, Visit(context.body));

		public override IASTObject VisitLoopControlStatement([NotNull] CParser.LoopControlStatementContext context)
		{
			if (context.Continue() is not null) return new ContinueStatement();
			else if (context.Break() is not null) return new BreakStatement();
			else throw new NotImplementedException();
        }

		public override IASTObject VisitSwitchStatement([NotNull] CParser.SwitchStatementContext context)
		{
			var cases = new OrderedDictionary<Int128, ScopeNode>();
			ScopeNode? def = null;
			var expr = Visit(context.expression());

			foreach (var i in context.switchBlock())
			{
				var block = new ScopeNode(false);
				block.Statements.AddRange(i.statement().Select(Visit));

				if (i.Default() is not null)
				{
					if (def is not null) throw new InvalidOperationException("Cannot have multiple default blocks");
					def = block;
				}
				else cases[(Int128)VisitInt(i.Constant()).Value] = block;
			}

			return new SwitchStatement(expr, cases, def);
        }

		public override IASTObject VisitStatement([NotNull] CParser.StatementContext context)
		{
			if (context.children[0].GetText() == ";") return new EmptyStatement();
			return Visit(context.children[0]);
		}

		#endregion

		#region Types

		public override IASTObject VisitType([NotNull] CParser.TypeContext context)
		{
			return Visit(context.abstractDeclarator(), Visit(context.declarationSpecifier()));
		}

		public override IASTObject VisitDeclarationSpecifier([NotNull] CParser.DeclarationSpecifierContext context)
		{
			TypeSpecifier t;

			if (context.builtinType() is CParser.BuiltinTypeContext b) t = (TypeSpecifier)Visit(b);
			else t = (TypeSpecifier)Visit(context.userType());

			t.IsConst = context.Const().Length != 0;
			t.IsExtern = context.Extern().Length != 0;
			t.IsTypedef = context.Typedef() is not null;

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
			if (context.Identifier() is ITerminalNode id) return UserTypes[id.GetText()];
			else return Visit(context.children[0]);
		}

		public override IASTObject VisitStructOrUnion([NotNull] CParser.StructOrUnionContext context)
		{
			if (context.structDeclaration().Length != 0)
			{
				var members = new List<DeclaratorPair>();

				foreach (var i in context.structDeclaration())
				{
					var baseType = Visit(i.declarationSpecifier());
					foreach (var decl in i.declarator())
					{
						members.Add(Visit(decl, baseType));
					}
				}

				var s = new StructTypeSpecifier(context.Identifier()?.GetText() ?? "", members);

				Structs[s.Name] = s;

				return s;
			}
			else return Structs[context.Identifier().GetText()];
		}

        #endregion

        #region Visit Overloads

        public TypeSpecifier Visit(CParser.TypeContext context) => (TypeSpecifier)Visit((IParseTree)context);
        public TypeSpecifier Visit(CParser.DeclarationSpecifierContext context) => (TypeSpecifier)Visit((IParseTree)context);
		public ScopeNode Visit(CParser.ScopeContext context) => (ScopeNode)Visit((IParseTree)context);
		public IExpression Visit(CParser.ExpressionContext context) => (IExpression)Visit((IParseTree)context);
		public UnresolvedLValue Visit(CParser.LvalueContext context) => (UnresolvedLValue)Visit((IParseTree)context);
		public Statement Visit(CParser.StatementContext context) => (Statement)Visit((IParseTree)context);
		public static ConstantExpression VisitInt(ITerminalNode node)
		{
			var text = node.GetText().ToLower();
			Int128 val;
			if (text.StartsWith('\''))
			{
				text = Parser.ProcessString(text.Trim('\''));
				if (text.Length != 1) throw new InvalidOperationException($"Invalid character literal \'{node.GetText()}\'");
				val = (Int128)Encoding.UTF8.GetBytes(text)[0];
				return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedChar), val);
			}
			else if (!text.Contains('.'))
			{
				bool unsigned = text.Contains('u');
				bool isLong = text.Contains('l');

				if (text.StartsWith("0x")) val = Int128.Parse(text.Replace("0x", "").Replace("u", "").Replace("l", ""), System.Globalization.NumberStyles.HexNumber);
				else val = Int128.Parse(text);

				if (unsigned && isLong) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedLong), val);
				else if (isLong) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.SignedLong), val);
				else if (unsigned) return new ConstantExpression(new BuiltinTypeSpecifier(BuiltinType.UnsignedInt), val);
				else return GetSmallestConstantExpression(val);
			}
			else throw new NotImplementedException();
		}

		public DeclaratorPair Visit(CParser.DeclaratorContext context, TypeSpecifier baseType)
		{
			DeclaratorPair pair;
			if (context.Star().Length == 0) pair = Visit(context.directDeclarator(), baseType);
			else
			{
				var t = new PointerTypeSpecifier(baseType);
				for (int i = 0; i < context.Star().Length - 1; i++)
				{
					t = new PointerTypeSpecifier(t);
				}
				pair = Visit(context.directDeclarator(), t);
			}
			pair.Type.IsConst = context.Const() is not null;
			return pair;
		}

		public DeclaratorPair Visit(CParser.DirectDeclaratorContext context, TypeSpecifier baseType)
		{
			if (context.Identifier() is ITerminalNode name) return new(baseType, name.GetText());
			else if (context.declarator() is CParser.DeclaratorContext decl) return Visit(decl, baseType);
			else if (context.LeftBracket() is not null) return Visit(context.directDeclarator(), new ArrayTypeSpecifier(baseType, (int)VisitInt(context.Constant()).Value));
			else return Visit(context.directDeclarator(), new FunctionTypeSpecifier(baseType, (ParameterList)Visit(context.parameterList())));
		}

		public TypeSpecifier Visit(CParser.AbstractDeclaratorContext context, TypeSpecifier baseType)
		{
			TypeSpecifier type;
			if (context.Star().Length == 0) type = baseType;
			else
			{
				type = new PointerTypeSpecifier(baseType);
				for (int i = 0; i < context.Star().Length - 1; i++)
				{
					type = new PointerTypeSpecifier(type);
				}
			}

			if (context.abstractDirectDeclarator() is CParser.AbstractDirectDeclaratorContext decl) type = Visit(decl, type);
			type.IsConst = context.Const() is not null;

			return type;
		}

		public TypeSpecifier Visit(CParser.AbstractDirectDeclaratorContext context, TypeSpecifier baseType)
		{
			if (context.abstractDeclarator() is CParser.AbstractDeclaratorContext decl) return Visit(decl, baseType);
			else if (context.LeftBracket() is not null) return Visit(context.abstractDirectDeclarator(), new ArrayTypeSpecifier(baseType, (int)VisitInt(context.Constant()).Value));
			else return Visit(context.abstractDirectDeclarator(), new FunctionTypeSpecifier(baseType, (ParameterList)Visit(context.parameterList())));
		}

		#endregion
	}
}
