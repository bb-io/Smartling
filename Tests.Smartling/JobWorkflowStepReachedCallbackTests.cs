using Apps.Smartling.Callbacks;
using Apps.Smartling.Callbacks.Models.Payload.Jobs;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Tests.Smartling;

[TestClass]
public class JobWorkflowStepReachedCallbackTests
{
    private const string DocumentedPayload = """
        {
          "eventId": "event-1",
          "eventType": "translationJob.localeWorkflowStep.reached",
          "schemaVersion": "1.0",
          "account": { "accountUid": "account-1", "accountName": "Test Account" },
          "project": { "projectUid": "project-1", "projectName": "Test Project" },
          "translationJob": { "jobUid": "job-1", "jobName": "Test Job", "jobStatus": "IN_PROGRESS" },
          "localeWorkflowStep": {
            "locale": { "localeId": "uk-UA", "description": "Ukrainian (Ukraine) [uk-UA]" },
            "reachedDate": "2026-01-15T18:00:01.416Z",
            "workflow": { "workflowUid": "workflow-1", "workflowName": "Test Workflow" },
            "workflowStep": { "workflowStepUid": "step-1", "workflowStepType": "TRANSLATION", "workflowStepName": "Translation" }
          }
        }
        """;

    private const string JobCreatedPayload = """
        {
          "eventId": "event-2",
          "eventType": "translationJob.created",
          "schemaVersion": "1.0",
          "account": { "accountUid": "account-1" },
          "project": { "projectUid": "project-1" },
          "translationJob": { "jobUid": "job-1" }
        }
        """;

    [TestMethod]
    public async Task OnJobWorkflowStepReachedManual_DocumentedPayloadFromFilteredProject_ReturnsEvent()
    {
        var response = await OnJobWorkflowStepReachedManual(DocumentedPayload, "project-1");

        Assert.AreNotEqual(WebhookRequestType.Preflight, response.ReceivedWebhookRequestType);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual("account-1", response.Result.Account.AccountUid);
        Assert.AreEqual("project-1", response.Result.Project.ProjectUid);
        Assert.AreEqual("job-1", response.Result.TranslationJob.JobUid);
        Assert.AreEqual("uk-UA", response.Result.LocaleWorkflowStep.Locale.LocaleId);
        Assert.AreEqual("step-1", response.Result.LocaleWorkflowStep.WorkflowStep.WorkflowStepUid);
    }

    [TestMethod]
    public async Task OnJobWorkflowStepReachedManual_PayloadFromAnotherProject_ReturnsPreflight()
    {
        var response = await OnJobWorkflowStepReachedManual(DocumentedPayload, "project-2");

        AssertPreflight(response);
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("{\"translationJobUid\":\"job-1\",\"type\":\"job.completed\"}")]
    [DataRow(JobCreatedPayload)]
    public async Task OnJobWorkflowStepReachedManual_BodyIsNotWorkflowStepEvent_ReturnsPreflight(string body)
    {
        var response = await OnJobWorkflowStepReachedManual(body, null);

        AssertPreflight(response);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("translationJobUid=job-1&type=job.completed")]
    [DataRow("{\"eventType\":")]
    public async Task OnJobWorkflowStepReachedManual_UnreadableBody_ReturnsPreflight(string body)
    {
        var response = await OnJobWorkflowStepReachedManual(body, null);

        AssertPreflight(response);
    }

    [TestMethod]
    public async Task OnJobWorkflowStepReached_PayloadFromAnotherProject_ReturnsPreflight()
    {
        var response = await new CallbackList(new InvocationContext()).OnJobWorkflowStepReached(
            new WebhookRequest { Body = DocumentedPayload },
            new ProjectIdentifier { ProjectId = "project-2" },
            new JobOptionalIdentifier(),
            new TargetLocalesIdentifier(),
            new WorkflowStepOptionalIdentifier());

        AssertPreflight(response);
    }

    private static Task<WebhookResponse<JobWorkflowStepReachedPayload>> OnJobWorkflowStepReachedManual(
        string body,
        string? projectId)
    {
        return new CallbackList(new InvocationContext()).OnJobWorkflowStepReachedManual(
            new WebhookRequest { Body = body },
            new ProjectIdentifier { ProjectId = projectId },
            new JobOptionalIdentifier(),
            new TargetLocalesIdentifier(),
            new WorkflowStepOptionalIdentifier());
    }

    private static void AssertPreflight(WebhookResponse<JobWorkflowStepReachedPayload> response)
    {
        Assert.AreEqual(WebhookRequestType.Preflight, response.ReceivedWebhookRequestType);
        Assert.IsNull(response.Result);
    }
}
