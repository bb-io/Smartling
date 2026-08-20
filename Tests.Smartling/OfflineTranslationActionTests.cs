using Apps.Smartling.Actions;
using Apps.Smartling.Models.Identifiers;
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

        // Act
        var result = await actions.CreateTranslationPackage(projectId, locale, workflowStep, job);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task DownloadXliffFromPackage_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new OfflineTranslationActions(context, FileManager);
        var projectId = new ProjectIdentifier { };
        var packageId = new TranslationPackageIdentifier { TranslationPackageUid = "50273b69" };

        // Act
        var result = await actions.DownloadXliffFromPackage(projectId, packageId);

        // Assert
        TestContext.Write(result.File.Name);
        Assert.IsNotNull(result);
    }

    [TestMethod, ContextDataSource]
    public async Task DownloadTmxFromPackage_IsSuccess(InvocationContext context)
    {
        // Arrange
        var actions = new OfflineTranslationActions(context, FileManager);
        var projectId = new ProjectIdentifier { };
        var packageId = new TranslationPackageIdentifier { TranslationPackageUid = "50273b69" };

        // Act
        var result = await actions.DownloadTmxFromPackage(projectId, packageId);

        // Assert
        TestContext.Write(result.File.Name);
        Assert.IsNotNull(result);
    }
}