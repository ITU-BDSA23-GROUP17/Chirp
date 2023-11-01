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
                //Remember to dotnet run before you run this test
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
                var querySelectorElementStrong = htmlDocument.QuerySelector("strong");
                if (querySelectorElementStrong == null)
                {
                        throw new Exception("QuerySelector returned null!");
                }
                string htmlStrong = querySelectorElementStrong.TextContent;
                string getName = htmlStrong.Replace(" ", "");
                getName = getName.Replace("\n", "");

                //Get the date, same principle as above
                var querySelectorElementSmall = htmlDocument.QuerySelector("small");
                if (querySelectorElementStrong == null)
                {
                        throw new Exception("QuerySelector returned null!");
                }
                string htmlSmall = querySelectorElementStrong.TextContent;
                string getDate = htmlSmall.Trim();
                getDate = getDate.Replace("\n", "");

                //Get the first message of Helge, same principle as above
                //Will need to replace Helge and Date, since they both are also in the p tag
                var querySelectorElementP = htmlDocument.QuerySelector("p");
                if (querySelectorElementStrong == null)
                {
                        throw new Exception("QuerySelector returned null!");
                }
                string htmlP = querySelectorElementStrong.TextContent;
                string getMessage = htmlP.Replace("\n", "");
                getMessage = getMessage.Trim();
                getMessage = getMessage.Replace(getName, "");
                getMessage = getMessage.Replace(getDate, "");
                getMessage = getMessage.Trim();

                Assert.Equal("Helge", getName);
                Assert.Equal("Hello, BDSA students!", getMessage);
                Assert.Equal("— 08/01/2023 12:16:48", getDate);
        }

        [Fact]
        public async void EndPointRasmusTest()
        {
                //Remember to dotnet run before you run this test
                //////// Arrange
                string stringURL = "https://bdsagroup17chirprazor.azurewebsites.net/Rasmus";
                using HttpClient client = new HttpClient();

                /////// Act
                HttpResponseMessage response = await client.GetAsync(stringURL);
                string documentContents = await response.Content.ReadAsStringAsync();

                /////// Assert
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

                // Parse the HTML document
                var parser = new HtmlParser();
                var htmlDocument = await parser.ParseDocumentAsync(documentContents);

                //get name Helge from the html. Since the name is inside <strong> we can query select it and remove all spaces and \n to only get his name
                var querySelectorElementStrong = htmlDocument.QuerySelector("strong");
                if (querySelectorElementStrong == null)
                {
                        throw new Exception("QuerySelector returned null!");
                }
                string htmlStrong = querySelectorElementStrong.TextContent;
                string getName = htmlStrong.Replace(" ", "");
                getName = getName.Replace("\n", "");

                //Get the date, same principle as above
                var querySelectorElementSmall = htmlDocument.QuerySelector("small");
                if (querySelectorElementSmall == null)
                {
                        throw new Exception("QuerySelector returned null!");
                }
                string htmlSmall = querySelectorElementSmall.TextContent;
                string getDate = htmlSmall.Trim();
                getDate = getDate.Replace("\n", "");

                //Get the first message of Helge, same principle as above
                //Will need to replace Helge and Date, since they both are also in the p tag
                var querySelectorElementP = htmlDocument.QuerySelector("p");
                if (querySelectorElementP == null)
                {
                        throw new Exception("QuerySelector returned null!");
                }
                string htmlP = querySelectorElementP.TextContent;
                string getMessage = htmlP.Replace("\n", "");
                getMessage = getMessage.Trim();
                getMessage = getMessage.Replace(getName, "");
                getMessage = getMessage.Replace(getDate, "");
                getMessage = getMessage.Trim();

                Assert.Equal("Rasmus", getName);
                Assert.Equal("Hej, velkommen til kurset.", getMessage);
                Assert.Equal("— 08/01/2023 13:08:28", getDate);

        }
}