using System.Diagnostics.Metrics;

namespace Chapter16;

public static class Metrics
{
    public static readonly Meter Meter = new("Chapter16");
}