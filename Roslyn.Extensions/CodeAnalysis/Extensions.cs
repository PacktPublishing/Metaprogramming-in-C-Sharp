using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn.Extensions.CodeAnalysis;

public static class Extensions
{
    public static bool InheritsASystemException(this ClassDeclarationSyntax classDeclaration, SemanticModel model)
    {
        var classSymbol = (model.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol)!;
        if (classSymbol.BaseType is null) return false;
        return classSymbol.BaseType.ContainingNamespace.Name.StartsWith("System", StringComparison.InvariantCulture) &&
            classSymbol.BaseType.Name.EndsWith("Exception", StringComparison.InvariantCulture);
    }

    public static bool IsAttribute(this ClassDeclarationSyntax classDeclaration, SemanticModel model)
    {
        var classSymbol = (model.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol)!;
        var baseType = classSymbol.BaseType;
        if (baseType is null)
        {
            return false;
        }

        while (!(baseType.ContainingNamespace.Name == "System" && baseType.Name == "Object"))
        {
            if (baseType.ContainingNamespace.Name.StartsWith("System", StringComparison.InvariantCulture) &&
                baseType.Name.EndsWith("Attribute", StringComparison.InvariantCulture))
            {
                return true;
            }

            baseType = baseType.BaseType;
            if (baseType is null) break;
        }

        return false;
    }
}
