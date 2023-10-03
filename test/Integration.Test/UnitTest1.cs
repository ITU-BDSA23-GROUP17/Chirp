namespace Integration.Test;

using System.Collections;
using System.Net;
using System.Web;

public class IntegrationTest
{
    [Fact]
    public async void TestHTTPRequest()
    {
        //Arrange
        string jsonUrl = "https://bdsagroup17chirpremotedb.azurewebsites.net/cheeps";
        using HttpClient client = new HttpClient();

        //Act
        HttpResponseMessage response = await client.GetAsync(jsonUrl);
        var jsonContent = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(jsonContent);
    }

    [Fact]
    public async void ReadEndPointUserHelge()
    {
        //Arrange
        string stringURL = "http://localhost:5273/Helge";
        using HttpClient client = new HttpClient();

        //Act
        HttpResponseMessage response = await client.GetAsync(stringURL);
        string documentContents = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(documentContents);
    }

}
