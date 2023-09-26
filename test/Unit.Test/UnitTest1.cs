using System.Collections;
namespace test;

public class UnitTest1
{
    [Fact]
    public void UnixTimeParserTest()
    {
        // Arrange
        var input = 1690978778;
        // Act
        var result = UserInterface.UnixTimeParser(input);
        // Assert
        Assert.Equal("8/2/2023 12:19:38 PM", result.ToString());
    }
}

