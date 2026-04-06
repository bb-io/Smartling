using Tests.Smartling.Base;
using Apps.Smartling.Actions;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Files;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Tests.Smartling;

[TestClass]
public class FileTests : TestBaseMultipleConnections
{
    [TestMethod, ContextDataSource(ConnectionTypes.AccountWide)]
    public async Task SearchFileTest_IsSuccess(InvocationContext context)
    {
        // Arrange
        var action = new FileActions(context, FileManager);
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var input = new SearchFilesRequest
        {
            //UploadedAfter = DateTime.UtcNow.AddDays(-1),
            //UploadedBefore = DateTime.UtcNow.AddDays(-1),
            //FileTypes = ["json"],
            //FileUriContains = "/fireboarding"
        };

        // Act
        var response = await action.SearchFiles(project, input);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.ProjectWide)]
    public async Task DownloadTranslatedFile_IsSuccess(InvocationContext invocationContext)
    {
        // Arrange
        var action = new FileActions(invocationContext, FileManager);
        var project = new ProjectIdentifier();
        var sourceFileIdentifier = new SourceFileIdentifier
        {
            FileUri = "Test static composition_en-US.html",
        };
        var targetLocaleIdentifier = new TargetLocaleIdentifier
        {
            TargetLocaleId = "fr-FR"
        };

        // Act
        var response = await action.DownloadTranslatedFile(project, sourceFileIdentifier, targetLocaleIdentifier);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }
}
