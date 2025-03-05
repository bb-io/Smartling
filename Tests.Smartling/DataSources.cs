using Apps.Smartling.DataSourceHandlers;
using SmartlingTests.Base;

namespace Tests.Smartling
{
    [TestClass]
    public class DataSources : TestBase
    {
        [TestMethod]
        public async Task FileDataHandlerReturnsValues()
        {
            var dataHandler = new FileDataSourceHandler(InvocationContext);

            var response = await dataHandler.GetDataAsync(new Blackbird.Applications.Sdk.Common.Dynamic.DataSourceContext { SearchString=""}, CancellationToken.None);
            foreach (var item in response)
            {
                Assert.IsNotNull(item);
                Console.WriteLine($"{item.Key} - {item.Value}");
            }
        }
    }
}
