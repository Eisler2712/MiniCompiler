using Compiler.Syntax;

namespace Compiler.Binding;

internal sealed class BoundDevelopFunction : BoundExpression
{
    public BoundDevelopFunction(BoundStatement expression, VariableSymbol name, BoundExpression value)
    {
        Expression = expression;
        Name = name;
        Value = value;
    }
    public BoundStatement Expression { get; }
    public VariableSymbol Name { get; }
    public BoundExpression Value { get; }
    public override BoundNodeKind Kind => BoundNodeKind.DevelopFunctionExpression;
    public override IEnumerable<BoundNode> GetChildren()
    {
        yield return Expression;
        yield return Value;
    }

    public override Type Type => Expression.GetType();
}