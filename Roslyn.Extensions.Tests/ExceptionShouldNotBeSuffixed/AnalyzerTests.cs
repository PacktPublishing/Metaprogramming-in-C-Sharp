namespace Roslyn.Extensions.CodeAnalysis.ExceptionShouldNotBeSuffixed;

using Xunit;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<ExceptionShouldNotBeSuffixed.Analyzer>;

public class AnalyzerTests
{
    [Fact]
    public async Task WithoutSuffix()
    {
        const string content = @"
                using System;

                namespace MyNamespace
                {
                    public class SomethingWentWrong : Exception
                    {

                    }
                }       
            ";

        await Verify.VerifyAnalyzerAsync(content);
    }

    [Fact]
    public async Task WithSuffix()
    {
        const string content = @"
                using System;

                namespace MyNamespace
                {
                    public class MyException : Exception
                    {

                    }
                }       
            ";

        var expected = Verify.Diagnostic().WithLocation(6, 34).WithArguments("MyException");
        await Verify.VerifyAnalyzerAsync(content, expected);
    }
}
