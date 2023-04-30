using Fundamentals.Metrics;

namespace ConsoleApp;

public static partial class ProgramMetrics
{
    [Counter<int>("Started", "How many times we've started the program")]
    public static partial void Started();
}


// public static partial class ProgramMetrics
// {
//     public static partial void Started()
//     {
        
//     }
// }