using Xunit;

namespace Chapter2;

public class CalculatorTests
{
    [Fact]
    public void Add()
    {
        // Arrange 
        var left = 5;
        var right = 3;
        var expectedResult = 8;

        // Act 
        var actualResult = Calculator.Add(left, right);
        Assert.Equal(expectedResult, actualResult);
    }
}