using Tests.Smartling.Base;
using Apps.Smartling.Constants;
using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Tests.Smartling;

[TestClass]
public class DataSources : TestBaseMultipleConnections
{
    [TestMethod, ContextDataSource]
    public async Task FileDataHandlerReturnsValues(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var dataHandler = new FileDataSourceHandler(context, project);

        // Act
        var response = await dataHandler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource]
    public async Task JobDataHandlerReturnsValues(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var dataHandler = new JobDataSourceHandler(context, project);

        // Act
        var response = await dataHandler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.AccountWide)]
    public async Task ProjectContextDataHandlerReturnsValues(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var dataHandler = new ProjectContextDataSourceHandler(context, project);

        // Act
        var response = await dataHandler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);
        
        // Assert
        PrintDataHandlerResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource]
    public async Task TargetLocaleDataHandlerReturnsValues(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var dataHandler = new TargetLocaleDataSourceHandler(context, project);

        // Act
        var response = await dataHandler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(response);
        Assert.IsNotNull(response);
    }
}
