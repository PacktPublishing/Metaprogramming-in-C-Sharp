namespace Roslyn.Extensions.CodeAnalysis.ExceptionShouldNotBeSuffixed;

using Xunit;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Analyzer, CodeFix>;

public class CodeFixTests
{
    [Fact]
    public async Task WithoutSuffix()
    {
        const string content = @"
                using System;

                namespace MyNamespace;
                public class SomethingWentWrong : Exception
                {
                }
            ";

        await Verify.VerifyCodeFixAsync(content, content);
    }

    [Fact]
    public async Task WithSuffix()
    {
        const string content = @"
                using System;

                namespace MyNamespace;
                public class MyException : Exception
                {
                }
            ";

        var expected = Verify.Diagnostic().WithLocation(5, 30).WithArguments("MyException");
        await Verify.VerifyCodeFixAsync(content, expected, content.Replace("MyException", "My"));
    }
}