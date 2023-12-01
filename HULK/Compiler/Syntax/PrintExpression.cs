namespace Compiler.Syntax;

public class PrintExpression : ExpressionSyntax
{
    public PrintExpression(SyntaxToken printKeyword, SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
    {
        PrintKeyword = printKeyword;
        OpenParenthesisToken = openParenthesisToken;
        Expression = expression;
        CloseParenthesisToken = closeParenthesisToken;
    }
    public SyntaxToken PrintKeyword { get; }
    public SyntaxToken OpenParenthesisToken { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken CloseParenthesisToken { get; }
    public override SyntaxKind Kind => SyntaxKind.PrintExpression;
   
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return PrintKeyword;
        yield return OpenParenthesisToken;
        yield return Expression;
        yield return CloseParenthesisToken;
    }
}