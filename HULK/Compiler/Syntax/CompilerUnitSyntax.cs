namespace Compiler.Syntax
{
    /// <summary>
    /// The CompilerUnitSyntax class represents the root of the syntax tree.
    /// </summary>
    public sealed class CompilerUnitSyntax : SyntaxNode
    {
        public CompilerUnitSyntax(StatementSyntax statement,SyntaxToken semicolonToken,SyntaxToken endOfFileToken)
        {
            Statement = statement;
            SemicolonToken = semicolonToken;
            EndOfFileToken = endOfFileToken;
        }

        public StatementSyntax Statement { get; }
        public SyntaxToken EndOfFileToken { get; }
        public SyntaxToken SemicolonToken { get; }

        public override SyntaxKind Kind => SyntaxKind.CompilerUnit;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Statement;
            yield return SemicolonToken;
            yield return EndOfFileToken;
        }
    }
}