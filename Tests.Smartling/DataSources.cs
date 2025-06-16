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

        [TestMethod]
        public async Task JobDataHandlerReturnsValues()
        {
            var dataHandler = new JobDataSourceHandler(InvocationContext);

            var response = await dataHandler.GetDataAsync(new Blackbird.Applications.Sdk.Common.Dynamic.DataSourceContext { SearchString = "" }, CancellationToken.None);
            foreach (var item in response)
            {
                Assert.IsNotNull(item);
                Console.WriteLine($"{item.Key} - {item.Value}");
            }
        }

        [TestMethod]
        public async Task ProjectDataHandlerReturnsValues()
        {
            var dataHandler = new ProjectContextDataSourceHandler(InvocationContext);

            var response = await dataHandler.GetDataAsync(new Blackbird.Applications.Sdk.Common.Dynamic.DataSourceContext { SearchString = "" }, CancellationToken.None);
            foreach (var item in response)
            {
                Assert.IsNotNull(item);
                Console.WriteLine($"{item.Key} - {item.Value}");
            }
        }

        [TestMethod]
        public async Task TargetLocaleDataHandlerReturnsValues()
        {
            var dataHandler = new TargetLocaleDataSourceHandler(InvocationContext);

            var response = await dataHandler.GetDataAsync(new Blackbird.Applications.Sdk.Common.Dynamic.DataSourceContext { SearchString = "" }, CancellationToken.None);
            foreach (var item in response)
            {
                Assert.IsNotNull(item);
                Console.WriteLine($"{item.Key} - {item.Value}");
            }
        }
    }
}
