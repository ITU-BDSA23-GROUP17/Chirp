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
        Assert.Equal("8/2/2023 12:19:38 PM", result.ToString());
    }
}
public class IntegrationTest1
{

    [Fact]
    public void Test1()
    {
        // Arange
        CSVDatabase<Cheep> database = new CSVDatabase<Cheep>();
        Cheep testCheep = new Cheep(Environment, "bla bla", 1234567890);

        // Act
        database.Store(testCheep);
        database.Read();
        // Assert 
        Assert.Contains(testCheep, database.Read());

    }
}