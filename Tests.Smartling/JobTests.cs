using Tests.Smartling.Base;
using Apps.Smartling.Actions;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Jobs;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Tests.Smartling;

[TestClass]
public class JobTests : TestBaseMultipleConnections
{
    [TestMethod, ContextDataSource]
    public async Task CreateJob_IsSuccess(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var action = new JobActions(context);
        var job = new CreateJobRequest { 
            JobName = "TestЕуіе", 
            CallbackUrl = "https://webhook.site/#!/view/46153d60-6c08-4081-9ab3-7ba5d527fec8/53e0cd33-1ad0-41dc-8000-3edd768d994b/1" 
        };
        var locale = new TargetLocalesIdentifier { /*TargetLocaleIds = ["fr-CA" ]*/};

        // Act
        var response = action.CreateJob(project, job, locale);

        // Assert
        foreach (var jobInfo in response.Result.TargetLocaleIds)
            TestContext.WriteLine($"{jobInfo}");

        TestContext.WriteLine($"{response.Result.ReferenceNumber}");
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource]
    public async Task AddStringJob_IsSuccess(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var action = new StringActions(context);
        var job = new JobIdentifier { TranslationJobUid = "zapjjrjaddrc" };
        var stringIdentifier = new StringIdentifier { Hashcode = "d2a79dda4a4ssfba3b62729003f7cccf" };
        var locale = new TargetLocalesIdentifier {  };

        // Act
        var response = await action.AddStringToJob(project, job, stringIdentifier, locale, false);

        // Assert
        Assert.IsNotNull(response);
    }

    [TestMethod, ContextDataSource]
    public async Task GetJob_IsSuccess(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var action = new JobActions(context);
        var job = new JobIdentifier { TranslationJobUid = "" };

        // Act
        var response = await action.GetJob(project, job);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }
}
