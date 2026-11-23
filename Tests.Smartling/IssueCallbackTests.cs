using Apps.Smartling.Callbacks;
using Apps.Smartling.Callbacks.Models.Payload.Issues;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Tests.Smartling;

[TestClass]
public class IssueCallbackTests
{
    private const string TranslationIssueCreatedPayload = """
        {
          "eventId": "event-1",
          "eventType": "translationIssue.created",
          "schemaVersion": "1.0",
          "account": { "accountUid": "account-1" },
          "project": { "projectUid": "project-1" },
          "translationIssue": {
            "translationIssueUid": "issue-1",
            "hashcode": "hash-1",
            "locale": { "localeId": "uk-UA" },
            "translationIssueText": "Wrong term",
            "createdDate": "2026-01-15T18:00:01Z"
          }
        }
        """;

    private const string SourceIssueCreatedPayload = """
        {
          "eventId": "event-2",
          "eventType": "sourceIssue.created",
          "schemaVersion": "1.0",
          "account": { "accountUid": "account-1" },
          "project": { "projectUid": "project-1" },
          "sourceIssue": { "sourceIssueUid": "issue-2", "hashcode": "hash-1", "sourceIssueText": "Typo" }
        }
        """;

    private const string TranslationIssueCommentCreatedPayload = """
        {
          "eventId": "event-3",
          "eventType": "translationIssue.comment.created",
          "schemaVersion": "1.0",
          "account": { "accountUid": "account-1" },
          "project": { "projectUid": "project-1" },
          "translationIssue": { "translationIssueUid": "issue-1", "hashcode": "hash-1" },
          "translationIssueComment": { "commentUid": "comment-1", "text": "Fixed" }
        }
        """;

    private const string SourceIssueCommentCreatedPayload = """
        {
          "eventId": "event-4",
          "eventType": "sourceIssue.comment.created",
          "schemaVersion": "1.0",
          "account": { "accountUid": "account-1" },
          "project": { "projectUid": "project-1" },
          "sourceIssue": { "sourceIssueUid": "issue-2", "hashcode": "hash-1" },
          "sourceIssueComment": { "commentUid": "comment-2", "text": "Fixed" }
        }
        """;

    private const string FilePublishedPayload = """
        {
          "eventId": "event-5",
          "eventType": "file.published",
          "schemaVersion": "1.0",
          "account": { "accountUid": "account-1" },
          "project": { "projectUid": "project-1" },
          "file": { "fileUri": "file.json", "publishedLocale": { "localeId": "uk-UA" } }
        }
        """;

    private const string TranslationIssueUpdatedPayload = """
        {
          "eventId": "event-6",
          "eventType": "translationIssue.updated",
          "schemaVersion": "1.0",
          "account": { "accountUid": "account-1" },
          "project": { "projectUid": "project-1" },
          "translationIssue": { "translationIssueUid": "issue-1", "hashcode": "hash-1" }
        }
        """;

    private static CallbackList Callbacks => new(new InvocationContext());

    [TestMethod]
    public async Task OnTranslationIssueCreatedManual_DocumentedPayloadFromFilteredProject_ReturnsEvent()
    {
        var response = await OnTranslationIssueCreatedManual(TranslationIssueCreatedPayload, "project-1");

        Assert.AreNotEqual(WebhookRequestType.Preflight, response.ReceivedWebhookRequestType);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual("account-1", response.Result.Account.AccountUid);
        Assert.AreEqual("project-1", response.Result.Project.ProjectUid);
        Assert.AreEqual("issue-1", response.Result.TranslationIssue.TranslationIssueUid);
        Assert.AreEqual("uk-UA", response.Result.TranslationIssue.Locale?.LocaleId);
    }

    [TestMethod]
    public async Task OnTranslationIssueCreatedManual_PayloadFromAnotherProject_ReturnsPreflight()
    {
        var response = await OnTranslationIssueCreatedManual(TranslationIssueCreatedPayload, "project-2");

        AssertPreflight(response);
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("{\"translationJobUid\":\"job-1\",\"type\":\"job.completed\"}")]
    [DataRow(FilePublishedPayload)]
    [DataRow(SourceIssueCreatedPayload)]
    [DataRow(TranslationIssueUpdatedPayload)]
    public async Task OnTranslationIssueCreatedManual_BodyIsNotTranslationIssueCreatedEvent_ReturnsPreflight(string body)
    {
        var response = await OnTranslationIssueCreatedManual(body, null);

        AssertPreflight(response);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("fileUri=file.json&locale=uk-UA")]
    [DataRow("{\"eventType\":")]
    public async Task OnTranslationIssueCreatedManual_UnreadableBody_ReturnsPreflight(string body)
    {
        var response = await OnTranslationIssueCreatedManual(body, null);

        AssertPreflight(response);
    }

    [TestMethod]
    public async Task OnTranslationIssueCreatedManual_IssueFilterMatches_ReturnsEvent()
    {
        var response = await Callbacks.OnTranslationIssueCreatedManual(
            new WebhookRequest { Body = TranslationIssueCreatedPayload },
            new ProjectIdentifier(),
            new IssueOptionalIdentifier { IssueUid = "issue-1" });

        Assert.AreEqual("issue-1", response.Result?.TranslationIssue.TranslationIssueUid);
    }

    [TestMethod]
    public async Task OnTranslationIssueCreated_PayloadFromAnotherProject_ReturnsPreflight()
    {
        var response = await Callbacks.OnTranslationIssueCreated(
            new WebhookRequest { Body = TranslationIssueCreatedPayload },
            new ProjectIdentifier { ProjectId = "project-2" },
            new IssueOptionalIdentifier());

        AssertPreflight(response);
    }

    [TestMethod]
    public async Task OnSourceIssueCreatedManual_DocumentedPayload_ReturnsEvent()
    {
        var response = await Callbacks.OnSourceIssueCreatedManual(
            new WebhookRequest { Body = SourceIssueCreatedPayload },
            new ProjectIdentifier { ProjectId = "project-1" },
            new IssueOptionalIdentifier());

        Assert.AreEqual("issue-2", response.Result?.SourceIssue.SourceIssueUid);
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow(TranslationIssueCreatedPayload)]
    public async Task OnSourceIssueCreatedManual_BodyIsNotSourceIssueCreatedEvent_ReturnsPreflight(string body)
    {
        var response = await Callbacks.OnSourceIssueCreatedManual(
            new WebhookRequest { Body = body },
            new ProjectIdentifier(),
            new IssueOptionalIdentifier());

        AssertPreflight(response);
    }

    [TestMethod]
    public async Task OnTranslationIssueCommentCreatedManual_DocumentedPayload_ReturnsEvent()
    {
        var response = await Callbacks.OnTranslationIssueCommentCreatedManual(
            new WebhookRequest { Body = TranslationIssueCommentCreatedPayload },
            new ProjectIdentifier { ProjectId = "project-1" },
            new IssueOptionalIdentifier(),
            new IssueCommentOptionalIdentifier());

        Assert.AreEqual("comment-1", response.Result?.TranslationIssueComment.CommentUid);
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow(TranslationIssueCreatedPayload)]
    public async Task OnTranslationIssueCommentCreatedManual_BodyIsNotCommentCreatedEvent_ReturnsPreflight(string body)
    {
        var response = await Callbacks.OnTranslationIssueCommentCreatedManual(
            new WebhookRequest { Body = body },
            new ProjectIdentifier(),
            new IssueOptionalIdentifier(),
            new IssueCommentOptionalIdentifier());

        AssertPreflight(response);
    }

    [TestMethod]
    public async Task OnSourceIssueCommentCreatedManual_DocumentedPayload_ReturnsEvent()
    {
        var response = await Callbacks.OnSourceIssueCommentCreatedManual(
            new WebhookRequest { Body = SourceIssueCommentCreatedPayload },
            new ProjectIdentifier { ProjectId = "project-1" },
            new IssueOptionalIdentifier(),
            new IssueCommentOptionalIdentifier());

        Assert.AreEqual("comment-2", response.Result?.SourceIssueComment.CommentUid);
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow(SourceIssueCreatedPayload)]
    public async Task OnSourceIssueCommentCreatedManual_BodyIsNotCommentCreatedEvent_ReturnsPreflight(string body)
    {
        var response = await Callbacks.OnSourceIssueCommentCreatedManual(
            new WebhookRequest { Body = body },
            new ProjectIdentifier(),
            new IssueOptionalIdentifier(),
            new IssueCommentOptionalIdentifier());

        AssertPreflight(response);
    }

    private static Task<WebhookResponse<TranslationIssueCreatedPayload>> OnTranslationIssueCreatedManual(
        string body,
        string? projectId)
    {
        return Callbacks.OnTranslationIssueCreatedManual(
            new WebhookRequest { Body = body },
            new ProjectIdentifier { ProjectId = projectId },
            new IssueOptionalIdentifier());
    }

    private static void AssertPreflight<T>(WebhookResponse<T> response) where T : class
    {
        Assert.AreEqual(WebhookRequestType.Preflight, response.ReceivedWebhookRequestType);
        Assert.IsNull(response.Result);
    }
}
