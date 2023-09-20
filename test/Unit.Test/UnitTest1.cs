using System.Collections;
namespace test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = 1690978778;
        // Act
        var result = UserInterface.UnixTimeParser(input);
        // Assert
        Assert.Equal("08/02/2023 12:19:38", result.ToString());
    }
}

