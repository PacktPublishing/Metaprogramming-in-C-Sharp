namespace Roslyn.Extensions.Metrics;

public class MetricsTemplateData
{
    public string Namespace { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public IEnumerable<CounterTemplateData> Counters { get; set; } = Enumerable.Empty<CounterTemplateData>();
}