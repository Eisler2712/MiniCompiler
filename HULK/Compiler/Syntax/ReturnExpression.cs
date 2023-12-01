namespace Compiler.Syntax;

public class ReturnExpression : ExpressionSyntax
{
    public ReturnExpression(SyntaxToken returnKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
    {
        ReturnKeyword = returnKeyword;
        Expression = expression;
        SemicolonToken = semicolonToken;
    }
    public SyntaxToken ReturnKeyword { get; }
    public ExpressionSyntax Expression { get; }
    
    public SyntaxToken SemicolonToken { get; }
    public override SyntaxKind Kind => SyntaxKind.ReturnExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Expression;
    }
}