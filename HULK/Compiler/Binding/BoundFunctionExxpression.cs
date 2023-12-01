using System.Collections.Immutable;
using Compiler.Syntax;

namespace Compiler.Binding;

internal sealed class BoundFunctionExpression : BoundExpression
{
    public BoundFunctionExpression(object expression, string name)
    {
        Expression = expression;
        Name = name;
        
    }
    public object Expression { get; }
    public string Name { get; }
    public override BoundNodeKind Kind => BoundNodeKind.FunctionExpression;
    public override IEnumerable<BoundNode> GetChildren()
    {
        yield break;
    }
    
    public override Type Type => Name.GetType();
   
}