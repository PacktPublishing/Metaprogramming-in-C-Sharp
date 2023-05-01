using System.Diagnostics.Metrics;

namespace Fundamentals.Metrics;

public static class GlobalMetrics
{
    public static Meter Meter = new("Global");
}