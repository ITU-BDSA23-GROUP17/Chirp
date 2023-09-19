using SimpleDB;

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
        Assert.Equal("8/2/2023 12:19:38", result.ToString());
    }
}
public class IntegrationTest1
{
    [Fact]
    public void Test1()
    {
        // Arange
        CSVDatabase<Cheep> database = CSVDatabase<Cheep>.Instance;
        var testCheep = new Cheep("Test", "bla bla", 1234567890);

        // Act
        database.Store(testCheep);

        var databaseCheeps = database.Read();

        // Assert 
        Assert.NotNull(databaseCheeps);
    }
}