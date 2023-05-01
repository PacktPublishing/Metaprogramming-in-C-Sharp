using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn.Extensions.GDPR;

[Generator]
public class GDPRSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not GDPRSyntaxReceiver receiver) return;

        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.GDPRReport", out var filename);

        var writer = File.CreateText(filename);
        writer.AutoFlush = true;

        var piiAttribute = context.Compilation.GetTypeByMetadataName("Fundamentals.Compliance.GDPR.PersonalIdentifiableInformationAttribute");
        foreach (var candidate in receiver.Candidates)
        {
            var memberNamesAndReasons = new List<(string MemberName, string Reason)>();

            var semanticModel = context.Compilation.GetSemanticModel(candidate.SyntaxTree);

            var symbols = new List<ISymbol>();

            if (candidate is RecordDeclarationSyntax record)
            {
                foreach (var parameter in record.ParameterList!.Parameters)
                {
                    var parameterSymbol = semanticModel.GetDeclaredSymbol(parameter);
                    if (parameterSymbol is not null)
                    {
                        symbols.Add(parameterSymbol);
                    }
                }
            }

            foreach (var member in candidate.Members)
            {
                if (member is not PropertyDeclarationSyntax property) continue;

                var propertySymbol = semanticModel.GetDeclaredSymbol(property);
                if (propertySymbol is not null)
                {
                    symbols.Add(propertySymbol);
                }
            }

            foreach (var symbol in symbols)
            {
                var attributes = symbol.GetAttributes();
                var attribute = attributes.FirstOrDefault(_ => SymbolEqualityComparer.Default.Equals(_.AttributeClass?.OriginalDefinition, piiAttribute));
                if (attribute is not null)
                {
                    memberNamesAndReasons.Add((symbol.Name, attribute.ConstructorArguments[0].Value!.ToString()));
                }
            }

            if (memberNamesAndReasons.Count > 0)
            {
                var @namespace = (candidate.Parent as BaseNamespaceDeclarationSyntax)!.Name.ToString();
                writer.WriteLine($"Type: {@namespace}.{candidate.Identifier.ValueText}");
                writer.WriteLine("Members:");
                foreach (var (memberName, reason) in memberNamesAndReasons)
                {
                    var reasonText = string.IsNullOrEmpty(reason) ? "No reason provided" : reason;
                    writer.WriteLine($"  {memberName}: {reasonText}");
                }

                writer.WriteLine(string.Empty);
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new GDPRSyntaxReceiver());
    }
}
