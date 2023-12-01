namespace Compiler.Syntax;

public class FunctionExpression:ExpressionSyntax
{
    public FunctionExpression(SyntaxToken keywordToken,SyntaxToken identifierToken, SyntaxToken openParenthesisToken, SyntaxToken variable, SyntaxToken closeParenthesisToken, SyntaxToken arrow, BlockStatementSyntax body,SyntaxToken semiColonToken)
    {
        KeywordToken = keywordToken;
        IdentifierToken = identifierToken;
        OpenParenthesisToken = openParenthesisToken;
        Variable = variable;
        CloseParenthesisToken = closeParenthesisToken;
        Arrow = arrow;
        Body = body;
        SemiColonToken = semiColonToken;
    }
    public SyntaxToken KeywordToken { get; }
    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken OpenParenthesisToken { get; }
    public SyntaxToken Variable { get; }
    public SyntaxToken CloseParenthesisToken { get; }
    public SyntaxToken Arrow { get; }
    public BlockStatementSyntax Body { get; }
    public SyntaxToken SemiColonToken { get; }
    public override SyntaxKind Kind => SyntaxKind.FunctionExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return KeywordToken;
        yield return IdentifierToken;
        yield return OpenParenthesisToken;
        yield return Variable;
        yield return CloseParenthesisToken;
        yield return Arrow;
        yield return Body;
        yield return SemiColonToken;
    }
}