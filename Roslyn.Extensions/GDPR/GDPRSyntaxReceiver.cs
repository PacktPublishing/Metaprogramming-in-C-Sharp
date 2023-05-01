using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn.Extensions.GDPR;

public class GDPRSyntaxReceiver : ISyntaxReceiver
{
    readonly List<TypeDeclarationSyntax> _candidates = new();

    internal IEnumerable<TypeDeclarationSyntax> Candidates => _candidates;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not TypeDeclarationSyntax typeSyntax) return;
        _candidates.Add(typeSyntax);
    }
}