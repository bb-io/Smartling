using Apps.Smartling.Actions;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.OfflineTranslations;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Smartling.Base;

namespace Tests.Smartling;

[TestClass]
public class OfflineTranslationActionTests : TestBaseMultipleConnections
{
    [TestMethod, ContextDataSource]
    public async Task CreateTranslationPackage_ReturnsPackageId(InvocationContext context)
    {
        // Arrange
        var actions = new OfflineTranslationActions(context, FileManager);
        var projectId = new ProjectIdentifier { };
        var locale = new TargetLocaleIdentifier { TargetLocaleId = "uk-UA" };
        var workflowStep = new WorkflowStepIdentifier { WorkflowStepUid = "1dd3ff77c392" };
        var job = new JobOptionalIdentifier { };
        var input = new CreateTranslationPackageRequest
        {
            
        };

        // Act
        var result = await actions.CreateTranslationPackage(projectId, locale, workflowStep, job, input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}