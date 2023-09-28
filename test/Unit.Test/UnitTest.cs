using System.Collections;
namespace test;

public class UnitTest
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var input = 1690978778;
        // Act
        var result = UserInterface.dateTime;
        // Assert
        Assert.Equal("02/08/2023 12:19:38", result.ToString());
    }
}

