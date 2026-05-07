using System.Net;
using Apps.Smartling.Callbacks.Handlers;
using Apps.Smartling.Callbacks.Models.Payload.Issues;
using Apps.Smartling.Callbacks.Models.Payload.Jobs;
using Apps.Smartling.Callbacks.Models.Payload.Strings;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Smartling.Callbacks;

[WebhookList]
public class CallbackList
{
    #region Bridge callbacks

    [Webhook("On job completed", typeof(JobCompletedCallbackHandler),
        Description = "This event is triggered when a job is completed.")]
    public async Task<WebhookResponse<JobCompletedPayload>> OnJobCompleted(
        WebhookRequest request,
        [WebhookParameter] JobOptionalIdentifier jobOptionalRequest,
        [WebhookParameter] ProjectIdentifier projectIdentifier)
    {
        var result = await HandleCallback<JobCompletedPayload>(request);
        if (jobOptionalRequest.TranslationJobUid != null && jobOptionalRequest.TranslationJobUid == result.Result?.TranslationJobUid)
        {
            return GetPreflightResponse<JobCompletedPayload>();
        }

        return result;
    }

    [Webhook("On job cancelled", typeof(JobCancelledCallbackHandler),
        Description = "This event is triggered when a job is cancelled.")]
    public async Task<WebhookResponse<JobCancelledPayload>> OnJobCancelled(
        WebhookRequest request,
        [WebhookParameter] JobOptionalIdentifier jobOptionalRequest,
        [WebhookParameter] ProjectIdentifier projectIdentifier)
    {
        var result = await HandleCallback<JobCancelledPayload>(request);
        if (jobOptionalRequest.TranslationJobUid != null && jobOptionalRequest.TranslationJobUid == result.Result?.TranslationJobUid)
        {
            return GetPreflightResponse<JobCancelledPayload>();
        }

        return result;
    }

    [Webhook("On string translation published", typeof(StringPublishedCallbackHandler),
        Description = "This event is triggered when a string translation is published for a locale.")]
    public async Task<WebhookResponse<StringPublishedPayload>> OnStringPublished(
        WebhookRequest request,
        [WebhookParameter] StringOptionalIdentifier optionalIdentifier)
    {
        var result = await HandleCallback<StringPublishedPayload>(request);
        if (optionalIdentifier.Hashcode != null && optionalIdentifier.Hashcode == result.Result?.Hashcode)
        {
            return GetPreflightResponse<StringPublishedPayload>();
        }

        return result;
    }

    [Webhook("On source issue created", typeof(SourceIssueCreatedCallbackHandler),
        Description = "This event is triggered when a source issue is created.")]
    public async Task<WebhookResponse<SourceIssueCreatedPayload>> OnSourceIssueCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIdentifier)
    {
        var result = await HandleCallback<SourceIssueCreatedPayload>(request);
        if (optionalIdentifier.IssueUid != null && optionalIdentifier.IssueUid != result.Result?.SourceIssue?.SourceIssueUid)
        {
            return GetPreflightResponse<SourceIssueCreatedPayload>();
        }

        return result;
    }

    [Webhook("On translation issue created", typeof(TranslationIssueCreatedCallbackHandler),
        Description = "This event is triggered when a translation issue is created.")]
    public async Task<WebhookResponse<TranslationIssueCreatedPayload>> OnTranslationIssueCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIdentifier)
    {
        var result = await HandleCallback<TranslationIssueCreatedPayload>(request);
        if (optionalIdentifier.IssueUid != null &&
            optionalIdentifier.IssueUid != result.Result?.TranslationIssue?.TranslationIssueUid)
        {
            return GetPreflightResponse<TranslationIssueCreatedPayload>();
        }

        return result;
    }

    [Webhook("On source issue comment created", typeof(SourceIssueCommentCreatedCallbackHandler),
        Description = "This event is triggered when a comment is added to a source issue.")]
    public async Task<WebhookResponse<SourceIssueCommentCreatedPayload>> OnSourceIssueCommentCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIssueIdentifier,
        [WebhookParameter] IssueCommentOptionalIdentifier optionalCommentIdentifier)
    {
        var result = await HandleCallback<SourceIssueCommentCreatedPayload>(request);
        if (optionalIssueIdentifier.IssueUid != null &&
            optionalIssueIdentifier.IssueUid != result.Result?.SourceIssue?.SourceIssueUid)
        {
            return GetPreflightResponse<SourceIssueCommentCreatedPayload>();
        }

        if (optionalCommentIdentifier.CommentUid != null &&
            optionalCommentIdentifier.CommentUid != result.Result?.SourceIssueComment?.CommentUid)
        {
            return GetPreflightResponse<SourceIssueCommentCreatedPayload>();
        }

        return result;
    }

    [Webhook("On translation issue comment created", typeof(TranslationIssueCommentCreatedCallbackHandler),
        Description = "This event is triggered when a comment is added to a translation issue.")]
    public async Task<WebhookResponse<TranslationIssueCommentCreatedPayload>> OnTranslationIssueCommentCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIssueIdentifier,
        [WebhookParameter] IssueCommentOptionalIdentifier optionalCommentIdentifier)
    {
        var result = await HandleCallback<TranslationIssueCommentCreatedPayload>(request);
        if (optionalIssueIdentifier.IssueUid != null &&
            optionalIssueIdentifier.IssueUid != result.Result?.TranslationIssue?.TranslationIssueUid)
        {
            return GetPreflightResponse<TranslationIssueCommentCreatedPayload>();
        }

        if (optionalCommentIdentifier.CommentUid != null &&
            optionalCommentIdentifier.CommentUid != result.Result?.TranslationIssueComment?.CommentUid)
        {
            return GetPreflightResponse<TranslationIssueCommentCreatedPayload>();
        }

        return result;
    }

    #endregion

    #region Manual callbacks

    [Webhook("On job completed (manual)", Description = "This manual event is triggered when a job is completed.")]
    public async Task<WebhookResponse<JobCompletedPayload>> OnJobCompletedManual(
        WebhookRequest request,
        [WebhookParameter] JobOptionalIdentifier jobOptionalRequest,
        [WebhookParameter] ProjectIdentifier projectIdentifier)
    {
        var result = await HandleCallback<JobCompletedPayload>(request);
        if (jobOptionalRequest.TranslationJobUid != null && jobOptionalRequest.TranslationJobUid == result.Result?.TranslationJobUid)
        {
            return GetPreflightResponse<JobCompletedPayload>();
        }

        return result;
    }

    [Webhook("On job cancelled (manual)", Description = "This manual event is triggered when a job is cancelled.")]
    public async Task<WebhookResponse<JobCancelledPayload>> OnJobCancelledManual(
        WebhookRequest request,
        [WebhookParameter] JobOptionalIdentifier jobOptionalRequest,
        [WebhookParameter] ProjectIdentifier projectIdentifier)
    {
        var result = await HandleCallback<JobCancelledPayload>(request);
        if (jobOptionalRequest.TranslationJobUid != null && jobOptionalRequest.TranslationJobUid == result.Result?.TranslationJobUid)
        {
            return GetPreflightResponse<JobCancelledPayload>();
        }

        return result;
    }

    [Webhook("On string translation published (manual)",
        Description = "This manual event is triggered when a string translation is published for a locale.")]
    public async Task<WebhookResponse<StringPublishedPayload>> OnStringPublishedManual(
        WebhookRequest request,
        [WebhookParameter] StringOptionalIdentifier optionalIdentifier)
    {
        var result = await HandleCallback<StringPublishedPayload>(request);
        if (optionalIdentifier.Hashcode != null && optionalIdentifier.Hashcode == result.Result?.Hashcode)
        {
            return GetPreflightResponse<StringPublishedPayload>();
        }

        return result;
    }

    [Webhook("On source issue created (manual)",
        Description = "This manual event is triggered when a source issue is created.")]
    public async Task<WebhookResponse<SourceIssueCreatedPayload>> OnSourceIssueCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIdentifier)
    {
        var result = await HandleCallback<SourceIssueCreatedPayload>(request);
        if (optionalIdentifier.IssueUid != null && optionalIdentifier.IssueUid != result.Result?.SourceIssue?.SourceIssueUid)
        {
            return GetPreflightResponse<SourceIssueCreatedPayload>();
        }

        return result;
    }

    [Webhook("On translation issue created (manual)",
        Description = "This manual event is triggered when a translation issue is created.")]
    public async Task<WebhookResponse<TranslationIssueCreatedPayload>> OnTranslationIssueCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIdentifier)
    {
        var result = await HandleCallback<TranslationIssueCreatedPayload>(request);
        if (optionalIdentifier.IssueUid != null &&
            optionalIdentifier.IssueUid != result.Result?.TranslationIssue?.TranslationIssueUid)
        {
            return GetPreflightResponse<TranslationIssueCreatedPayload>();
        }

        return result;
    }

    [Webhook("On source issue comment created (manual)",
        Description = "This manual event is triggered when a comment is added to a source issue.")]
    public async Task<WebhookResponse<SourceIssueCommentCreatedPayload>> OnSourceIssueCommentCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIssueIdentifier,
        [WebhookParameter] IssueCommentOptionalIdentifier optionalCommentIdentifier)
    {
        var result = await HandleCallback<SourceIssueCommentCreatedPayload>(request);
        if (optionalIssueIdentifier.IssueUid != null &&
            optionalIssueIdentifier.IssueUid != result.Result?.SourceIssue?.SourceIssueUid)
        {
            return GetPreflightResponse<SourceIssueCommentCreatedPayload>();
        }

        if (optionalCommentIdentifier.CommentUid != null &&
            optionalCommentIdentifier.CommentUid != result.Result?.SourceIssueComment?.CommentUid)
        {
            return GetPreflightResponse<SourceIssueCommentCreatedPayload>();
        }

        return result;
    }

    [Webhook("On translation issue comment created (manual)",
        Description = "This manual event is triggered when a comment is added to a translation issue.")]
    public async Task<WebhookResponse<TranslationIssueCommentCreatedPayload>> OnTranslationIssueCommentCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIssueIdentifier,
        [WebhookParameter] IssueCommentOptionalIdentifier optionalCommentIdentifier)
    {
        var result = await HandleCallback<TranslationIssueCommentCreatedPayload>(request);
        if (optionalIssueIdentifier.IssueUid != null &&
            optionalIssueIdentifier.IssueUid != result.Result?.TranslationIssue?.TranslationIssueUid)
        {
            return GetPreflightResponse<TranslationIssueCommentCreatedPayload>();
        }

        if (optionalCommentIdentifier.CommentUid != null &&
            optionalCommentIdentifier.CommentUid != result.Result?.TranslationIssueComment?.CommentUid)
        {
            return GetPreflightResponse<TranslationIssueCommentCreatedPayload>();
        }

        return result;
    }

    #endregion

    private Task<WebhookResponse<T>> HandleCallback<T>(WebhookRequest request) where T : class
    {
        var payload = JsonConvert.DeserializeObject<T>(request.Body.ToString(),
            new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

        return Task.FromResult(new WebhookResponse<T>
        {
            HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
            Result = payload
        });
    }

    private WebhookResponse<T> GetPreflightResponse<T>() where T : class
    {
        return new WebhookResponse<T>
        {
            HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
            Result = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        };
    }
}
