namespace Integration.Test;
using System.Collections;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;

public class IntegrationTest
{
        [Fact]
        public async void EndPointHelgeTest()
        {
                /*         //Remember to dotnet run before you run this test
                        //////// Arrange
                        string stringURL = "https://bdsagroup17chirprazor.azurewebsites.net/Helge";
                        using HttpClient client = new HttpClient();

                        //////// Act
                        HttpResponseMessage response = await client.GetAsync(stringURL);
                        string documentContents = await response.Content.ReadAsStringAsync();

                        //////// Assert
                        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

                        // Parse the HTML document
                        var parser = new HtmlParser();
                        var htmlDocument = await parser.ParseDocumentAsync(documentContents);

                        //get name Helge from the html. Since the name is inside <strong> we can query select it and remove all spaces and \n to only get his name
                        var querySelectorElementStrong = htmlDocument.QuerySelector("strong").TextContent.Trim().Replace("\n", "");
                        if (querySelectorElementStrong == null)
                        {
                                throw new Exception("QuerySelector returned null!");
                        }

                        //get message by Helge from the html. Since the name is inside <strong> we can query select it and remove all spaces and \n to only get his name
                        var querySelectorElementSmall = htmlDocument.QuerySelector("small").TextContent.Trim().Replace("\n", "");
                        if (querySelectorElementStrong == null)
                        {
                                throw new Exception("QuerySelector returned null!");
                        }

                        Assert.Equal("Hello, BDSA students!", querySelectorElementSmall); */
        }
}