using System.Diagnostics.Metrics;

namespace Fundamentals.Metrics;

public static class GlobalMetrics
{
    public static readonly Meter Meter = new("Global");
}