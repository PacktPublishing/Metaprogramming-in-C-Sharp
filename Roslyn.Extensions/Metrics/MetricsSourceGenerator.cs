using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Extensions.Templates;

namespace Roslyn.Extensions.Metrics;

[Generator]
public class MetricsSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not MetricsSyntaxReceiver receiver) return;

        // while (!System.Diagnostics.Debugger.IsAttached) Thread.Sleep(10);
        var counterAttribute = context.Compilation.GetTypeByMetadataName("Fundamentals.Metrics.CounterAttribute`1");
        foreach (var candidate in receiver.Candidates)
        {
            var semanticModel = context.Compilation.GetSemanticModel(candidate.SyntaxTree);
            foreach (var member in candidate.Members)
            {
                if (member is not MethodDeclarationSyntax method) continue;

                var methodSymbol = semanticModel.GetDeclaredSymbol(method);
                if (methodSymbol is not null)
                {
                    var attributes = methodSymbol.GetAttributes();
                    var attribute = attributes.FirstOrDefault(_ => SymbolEqualityComparer.Default.Equals(_.AttributeClass?.OriginalDefinition, counterAttribute));
                    if (attribute is not null)
                    {
                        var name = attribute.ConstructorArguments[0].Value!.ToString();
                        var description = attribute.ConstructorArguments[1].Value!.ToString();

                        var templateData = new MetricsTemplateData
                        {
                            Namespace = "ConsoleApp",
                            ClassName = "ProgramMetrics",
                            Counters = new[]
                            {
                                new CounterTemplateData
                                {
                                    Name = name,
                                    Description = description,
                                    Type = "int",
                                    MethodName = method.Identifier.ValueText,

                                    // Tags = new[]
                                    // {
                                    //     new CounterTagTemplateData
                                    //     {
                                    //         Name = "tag1",
                                    //         Type = "string"
                                    //     },
                                    //     new CounterTagTemplateData
                                    //     {
                                    //         Name = "tag2",
                                    //         Type = "string"
                                    //     }
                                    // }
                                }
                            }
                        };

                        var source = TemplateTypes.Metrics(templateData);
                        context.AddSource($"{candidate.Identifier.ValueText}.g.cs", source);
                    }
                }
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new MetricsSyntaxReceiver());
    }
}
