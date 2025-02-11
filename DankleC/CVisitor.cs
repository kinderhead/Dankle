//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /home/daniel/Documents/CSharp/Dankle/DankleC/C.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace DankleC {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="CParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface ICVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRoot([NotNull] CParser.RootContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunction([NotNull] CParser.FunctionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.scope"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitScope([NotNull] CParser.ScopeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] CParser.StatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.semiStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSemiStatement([NotNull] CParser.SemiStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.initAssignmentStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInitAssignmentStatement([NotNull] CParser.InitAssignmentStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.assignmentStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignmentStatement([NotNull] CParser.AssignmentStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.declareStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeclareStatement([NotNull] CParser.DeclareStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.relationalExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRelationalExpression([NotNull] CParser.RelationalExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.equalityExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEqualityExpression([NotNull] CParser.EqualityExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.logicalAndExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLogicalAndExpression([NotNull] CParser.LogicalAndExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.logicalOrExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLogicalOrExpression([NotNull] CParser.LogicalOrExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.assignmentExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignmentExpression([NotNull] CParser.AssignmentExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.indexExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIndexExpression([NotNull] CParser.IndexExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturnStatement([NotNull] CParser.ReturnStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.lvalue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLvalue([NotNull] CParser.LvalueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression([NotNull] CParser.ExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.castExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCastExpression([NotNull] CParser.CastExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.unaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnaryExpression([NotNull] CParser.UnaryExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.postfixExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPostfixExpression([NotNull] CParser.PostfixExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.primaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPrimaryExpression([NotNull] CParser.PrimaryExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.constantExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConstantExpression([NotNull] CParser.ConstantExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.additiveExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAdditiveExpression([NotNull] CParser.AdditiveExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.multiplicativeExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMultiplicativeExpression([NotNull] CParser.MultiplicativeExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.variableExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariableExpression([NotNull] CParser.VariableExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType([NotNull] CParser.TypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.userType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUserType([NotNull] CParser.UserTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.builtinType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBuiltinType([NotNull] CParser.BuiltinTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.integerType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIntegerType([NotNull] CParser.IntegerTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.unsignedChar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnsignedChar([NotNull] CParser.UnsignedCharContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.signedChar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSignedChar([NotNull] CParser.SignedCharContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.unsignedShort"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnsignedShort([NotNull] CParser.UnsignedShortContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.signedShort"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSignedShort([NotNull] CParser.SignedShortContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.unsignedInt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnsignedInt([NotNull] CParser.UnsignedIntContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.signedInt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSignedInt([NotNull] CParser.SignedIntContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.unsignedLong"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnsignedLong([NotNull] CParser.UnsignedLongContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.signedLong"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSignedLong([NotNull] CParser.SignedLongContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.unsignedLongLong"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnsignedLongLong([NotNull] CParser.UnsignedLongLongContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.signedLongLong"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSignedLongLong([NotNull] CParser.SignedLongLongContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.float"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFloat([NotNull] CParser.FloatContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.double"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDouble([NotNull] CParser.DoubleContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.longDouble"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLongDouble([NotNull] CParser.LongDoubleContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="CParser.lineMarker"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLineMarker([NotNull] CParser.LineMarkerContext context);
}
} // namespace DankleC
