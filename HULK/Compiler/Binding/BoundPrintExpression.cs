namespace Compiler.Binding;

internal sealed class BoundPrintExpression: BoundExpression
{
    public BoundPrintExpression(BoundExpression expression)
    {
        Expression = expression;
    }

    public BoundExpression Expression { get;}
    public override BoundNodeKind Kind => BoundNodeKind.PrintExpression;
    public override Type Type => Expression.GetType();
    public override IEnumerable<BoundNode> GetChildren()
    {
        yield break;
    }

}