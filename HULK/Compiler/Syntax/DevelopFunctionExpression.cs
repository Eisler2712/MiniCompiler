namespace Compiler.Syntax;

public class DevelopFunctionExpression : ExpressionSyntax
{
    public DevelopFunctionExpression(SyntaxToken identifierToken, SyntaxToken openParenthesisToken, ExpressionSyntax variable, SyntaxToken closeParenthesisToken)
    {
        IdentifierToken = identifierToken;
        OpenParenthesisToken = openParenthesisToken;
        Variable = variable;
        CloseParenthesisToken = closeParenthesisToken;
    }
    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken OpenParenthesisToken { get; }
    public ExpressionSyntax Variable { get; }
    public SyntaxToken CloseParenthesisToken { get; }
    public override SyntaxKind Kind => SyntaxKind.DevelopFunctionExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IdentifierToken;
        yield return OpenParenthesisToken;
        yield return Variable;
        yield return CloseParenthesisToken;
    }
}