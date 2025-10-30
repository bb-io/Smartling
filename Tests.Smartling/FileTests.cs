using Apps.Smartling.Actions;
using Apps.Smartling.Models.Requests.Files;
using SmartlingTests.Base;

namespace Tests.Smartling
{
    [TestClass]
    public class FileTests : TestBase
    {
        [TestMethod]
        public async Task SearchFileTest_IsSuccess()
        {
            var action = new FileActions(InvocationContext, FileManager);

            var input = new SearchFilesRequest
            {
                //UploadedAfter = DateTime.UtcNow.AddDays(-1),
                //UploadedBefore = DateTime.UtcNow.AddDays(-1),
                //FileTypes = ["json"],
                //FileUriContains = "/fireboarding"
            };

            var response = await action.SearchFiles(input);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);

            Assert.IsNotNull(response);
        }
    }
}
