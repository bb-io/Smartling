using Apps.Smartling.Actions;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Smartling.Base;

namespace Tests.Smartling;

[TestClass]
public class AttachmentActionTests : TestBaseMultipleConnections
{
    [TestMethod, ContextDataSource(ConnectionTypes.ProjectWide)]
    public async Task DownloadJobAttachment_IsSuccess(InvocationContext invocationContext)
    {
        // Arrange
        var actions = new AttachmentActions(invocationContext, FileManager);
        
        var job = new JobIdentifier { TranslationJobUid = "z7cfvvstfwwu" };
        var project = new ProjectIdentifier { };
        var attachment = new JobAttachmentIdentifier { AttachmentUid = "e690db13b52e" };

        // Act
        var result = await actions.DownloadJobAttachment(job, project, attachment);

        // Assert
        TestContext.WriteLine(result.File.Name);
    }
}