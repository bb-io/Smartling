using Tests.Smartling.Base;
using System.Globalization;
using Apps.Smartling.Polling;
using Apps.Smartling.Polling.Models;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Tests.Smartling;

[TestClass]
public class PollingTests : TestBaseMultipleConnections
{
    [TestMethod, ContextDataSource(ConnectionTypes.ProjectWide)]
    public async Task OnJobCompleted(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = string.Empty };
        var action = new PollingList(context);
        var customDate = DateTime.ParseExact("Mar 27, 2025, 6:31 PM", "MMM dd, yyyy, h:mm tt", CultureInfo.InvariantCulture);
        var request = new PollingEventRequest<DateMemory>
        {
            Memory = new DateMemory { LastInteractionDate = customDate }
        };

        // Act
        var response = await action.OnJobsCompleted(request, project);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource]
    public async Task OnJobAuthorized(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var customDate = DateTime.UtcNow.AddDays(-1);
        var action = new PollingList(context);
        var request = new PollingEventRequest<DateMemory>
        {
            Memory = new DateMemory { LastInteractionDate = customDate }
        };

        // Act
        var response = await action.OnJobsCompleted(request, project);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }
}
