using Tests.Smartling.Base;
using Apps.Smartling.Actions;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Files;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using System.Net.Mime;
using Apps.Smartling.Models;

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

    [TestMethod, ContextDataSource(ConnectionTypes.ProjectWide)]
    public async Task DownloadTranslatedFile_AsXliffFromSandbox_IsSuccess(InvocationContext invocationContext)
    {
        // Arrange
        var action = new FileActions(invocationContext, FileManager);
        var project = new ProjectIdentifier
        {
            ProjectId = "ca8b02a02"
        };
        var sourceFileIdentifier = new SourceFileIdentifier
        {
            FileUri = "MyRepairs.Online_en-US_fr-FR1.html.xlf",
        };
        var targetLocaleIdentifier = new TargetLocaleIdentifier
        {
            TargetLocaleId = "da"
        };

        // Act
        var response = await action.DownloadTranslatedFile(project, sourceFileIdentifier, targetLocaleIdentifier);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.ProjectWide)]
    public async Task UploadFile_AsXliff_IsSuccess(InvocationContext invocationContext)
    {
        // Arrange
        var action = new FileActions(invocationContext, FileManager);
        var project = new ProjectIdentifier { ProjectId = "ca8b02a02" };
        var file = new FileWrapper
        {
            File = await FileManager.UploadTestFileAsync("MyRepairs.Online_en-US_fr-FR1.html", MediaTypeNames.Text.Html)
        };
        var input = new fileUploadRequest
        {
            Type = "html",
            UploadAsXliff = true
        };

        // Act
        var response = await action.UploadFile(project, file, input);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource(ConnectionTypes.ProjectWide)]
    public async Task GetGlossaryEntry_IsSuccess(InvocationContext invocationContext)
    {
        // Arrange
        var action = new GlossaryActions(invocationContext, FileManager);
        var project = new GlossaryIdentifier
        {
            GlossaryUid = "e0f0e418-5f42-4a62-8b16-d913fa7d08ae"
        };
        var sourceFileIdentifier = new GlossaryEntryIdentifier
        {
            EntryUid = "30758fd2-246e-4662-932e-18446f39c126"
        };
    

        // Act
        var response = await action.GetGlossaryEntry(project, sourceFileIdentifier);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }
}
