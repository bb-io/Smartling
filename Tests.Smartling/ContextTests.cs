using Tests.Smartling.Base;
using Apps.Smartling.Actions;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Context;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Tests.Smartling;

[TestClass]
public class ContextTests : TestBaseMultipleConnections
{
    [TestMethod, ContextDataSource]
    public async Task UploadNewContext_IsSuccess(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var action = new ContextActions(context, FileManager);
        var request = new AddProjectContextRequest
        {
            ContextFile = new FileReference { Name = "test.MOV" },
            Name = "FirstFileMOV",
            ContextMatching = true
        };

        // Act
        var response = await action.UploadNewContext(project, request);

        // Assert
        TestContext.WriteLine(response.Response.Code);
        TestContext.WriteLine(response.Response.Data.ContextUid);
        TestContext.WriteLine(response.Response.Data.ProcessUid);
        Assert.IsNotNull(response);
    }
}
