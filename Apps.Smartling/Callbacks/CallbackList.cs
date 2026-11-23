using System.Net;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Apps.Smartling.Api;
using Apps.Smartling.Callbacks.Handlers;
using Apps.Smartling.Callbacks.Models.Payload.Files;
using Apps.Smartling.Callbacks.Models.Payload.Issues;
using Apps.Smartling.Callbacks.Models.Payload.Jobs;
using Apps.Smartling.Callbacks.Models.Payload.Strings;
using Apps.Smartling.Models.Dtos.Files;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Smartling.Callbacks;

[WebhookList]
public class CallbackList(InvocationContext invocationContext) : SmartlingInvocable(invocationContext)
{
    private const string JobWorkflowStepReachedEventType = "translationJob.localeWorkflowStep.reached";
    private const string SourceIssueCreatedEventType = "sourceIssue.created";
    private const string TranslationIssueCreatedEventType = "translationIssue.created";
    private const string SourceIssueCommentCreatedEventType = "sourceIssue.comment.created";
    private const string TranslationIssueCommentCreatedEventType = "translationIssue.comment.created";

    #region Bridge callbacks

    [Webhook("On job created", typeof(JobCreatedCallbackHandler),
        Description = "This event is triggered when a job is created.")]
    public async Task<WebhookResponse<JobCreatedPayload>> OnJobCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier)
    {
        var result = await HandleCallback<JobCreatedPayload>(request);
        return MatchesProjectFilter(result.Result, projectIdentifier)
            ? result
            : GetPreflightResponse<JobCreatedPayload>();
    }

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

    [Webhook("On job workflow step reached", typeof(JobWorkflowStepReachedCallbackHandler),
        Description = "This event is triggered when all authorized content in a job for a locale reaches a workflow step.")]
    public Task<WebhookResponse<JobWorkflowStepReachedPayload>> OnJobWorkflowStepReached(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] JobOptionalIdentifier jobOptionalIdentifier,
        [WebhookParameter] TargetLocalesIdentifier targetLocalesIdentifier,
        [WebhookParameter] WorkflowStepOptionalIdentifier workflowStepOptionalIdentifier)
    {
        return HandleJobWorkflowStepReachedCallback(
            request,
            projectIdentifier,
            jobOptionalIdentifier,
            targetLocalesIdentifier,
            workflowStepOptionalIdentifier);
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
    public Task<WebhookResponse<SourceIssueCreatedPayload>> OnSourceIssueCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIdentifier)
    {
        return HandleSourceIssueCreatedCallback(request, projectIdentifier, optionalIdentifier);
    }

    [Webhook("On translation issue created", typeof(TranslationIssueCreatedCallbackHandler),
        Description = "This event is triggered when a translation issue is created.")]
    public Task<WebhookResponse<TranslationIssueCreatedPayload>> OnTranslationIssueCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIdentifier)
    {
        return HandleTranslationIssueCreatedCallback(request, projectIdentifier, optionalIdentifier);
    }

    [Webhook("On source issue comment created", typeof(SourceIssueCommentCreatedCallbackHandler),
        Description = "This event is triggered when a comment is added to a source issue.")]
    public Task<WebhookResponse<SourceIssueCommentCreatedPayload>> OnSourceIssueCommentCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIssueIdentifier,
        [WebhookParameter] IssueCommentOptionalIdentifier optionalCommentIdentifier)
    {
        return HandleSourceIssueCommentCreatedCallback(request, projectIdentifier, optionalIssueIdentifier,
            optionalCommentIdentifier);
    }

    [Webhook("On translation issue comment created", typeof(TranslationIssueCommentCreatedCallbackHandler),
        Description = "This event is triggered when a comment is added to a translation issue.")]
    public Task<WebhookResponse<TranslationIssueCommentCreatedPayload>> OnTranslationIssueCommentCreated(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIssueIdentifier,
        [WebhookParameter] IssueCommentOptionalIdentifier optionalCommentIdentifier)
    {
        return HandleTranslationIssueCommentCreatedCallback(request, projectIdentifier, optionalIssueIdentifier,
            optionalCommentIdentifier);
    }

    #endregion

    #region Manual callbacks

    [Webhook("On job created (manual)", Description = "This manual event is triggered when a job is created.")]
    public async Task<WebhookResponse<JobCreatedPayload>> OnJobCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier)
    {
        var result = await HandleCallback<JobCreatedPayload>(request);
        return MatchesProjectFilter(result.Result, projectIdentifier)
            ? result
            : GetPreflightResponse<JobCreatedPayload>();
    }

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

    [Webhook("On job workflow step reached (manual)",
        Description = "This manual event is triggered when all authorized content in a job for a locale reaches a workflow step.")]
    public Task<WebhookResponse<JobWorkflowStepReachedPayload>> OnJobWorkflowStepReachedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] JobOptionalIdentifier jobOptionalIdentifier,
        [WebhookParameter] TargetLocalesIdentifier targetLocalesIdentifier,
        [WebhookParameter] WorkflowStepOptionalIdentifier workflowStepOptionalIdentifier)
    {
        return HandleJobWorkflowStepReachedCallback(
            request,
            projectIdentifier,
            jobOptionalIdentifier,
            targetLocalesIdentifier,
            workflowStepOptionalIdentifier);
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
    public Task<WebhookResponse<SourceIssueCreatedPayload>> OnSourceIssueCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIdentifier)
    {
        return HandleSourceIssueCreatedCallback(request, projectIdentifier, optionalIdentifier);
    }

    [Webhook("On translation issue created (manual)",
        Description = "This manual event is triggered when a translation issue is created.")]
    public Task<WebhookResponse<TranslationIssueCreatedPayload>> OnTranslationIssueCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIdentifier)
    {
        return HandleTranslationIssueCreatedCallback(request, projectIdentifier, optionalIdentifier);
    }

    [Webhook("On source issue comment created (manual)",
        Description = "This manual event is triggered when a comment is added to a source issue.")]
    public Task<WebhookResponse<SourceIssueCommentCreatedPayload>> OnSourceIssueCommentCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIssueIdentifier,
        [WebhookParameter] IssueCommentOptionalIdentifier optionalCommentIdentifier)
    {
        return HandleSourceIssueCommentCreatedCallback(request, projectIdentifier, optionalIssueIdentifier,
            optionalCommentIdentifier);
    }

    [Webhook("On file published (manual)",
        Description = "This manual event is triggered when all authorized content in a file reaches the Published workflow step in a locale.")]
    public async Task<WebhookResponse<FilePublishedPayload>> OnFilePublishedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] TargetLocaleOptionalIdentifier targetLocaleIdentifier,
        [WebhookParameter] SourceFileOptionalIdentifier sourceFileOptionalIdentifier,
        [WebhookParameter] [Display("Job name contains")] string? jobNameContains)
    {
        var result = HandleFilePublishedCallback(request);

        if (!string.Equals(result.Result?.PublishStatus, "published", StringComparison.OrdinalIgnoreCase))
            return GetPreflightResponse<FilePublishedPayload>();

        if (!string.IsNullOrWhiteSpace(targetLocaleIdentifier.TargetLocaleId) &&
            !string.Equals(targetLocaleIdentifier.TargetLocaleId, result.Result?.Locale, StringComparison.OrdinalIgnoreCase))
        {
            return GetPreflightResponse<FilePublishedPayload>();
        }

        if (!string.IsNullOrWhiteSpace(sourceFileOptionalIdentifier.FileUri) &&
            !string.Equals(sourceFileOptionalIdentifier.FileUri, result.Result?.FileUri, StringComparison.OrdinalIgnoreCase))
        {
            return GetPreflightResponse<FilePublishedPayload>();
        }

        if (!string.IsNullOrWhiteSpace(jobNameContains))
        {
            if (!ContainsFormattingIndifferent(result.Result?.JobName, jobNameContains))
                return GetPreflightResponse<FilePublishedPayload>();
        }

        if (!await IsFileFullyPublished(projectIdentifier, result.Result?.FileUri, result.Result?.Locale))
            return GetPreflightResponse<FilePublishedPayload>();

        return result;
    }

    [Webhook("On translation issue comment created (manual)",
        Description = "This manual event is triggered when a comment is added to a translation issue.")]
    public Task<WebhookResponse<TranslationIssueCommentCreatedPayload>> OnTranslationIssueCommentCreatedManual(
        WebhookRequest request,
        [WebhookParameter] ProjectIdentifier projectIdentifier,
        [WebhookParameter] IssueOptionalIdentifier optionalIssueIdentifier,
        [WebhookParameter] IssueCommentOptionalIdentifier optionalCommentIdentifier)
    {
        return HandleTranslationIssueCommentCreatedCallback(request, projectIdentifier, optionalIssueIdentifier,
            optionalCommentIdentifier);
    }

    #endregion

    private Task<WebhookResponse<SourceIssueCreatedPayload>> HandleSourceIssueCreatedCallback(
        WebhookRequest request,
        ProjectIdentifier projectIdentifier,
        IssueOptionalIdentifier issueIdentifier)
    {
        return HandleIssueCallback<SourceIssueCreatedPayload>(
            request,
            SourceIssueCreatedEventType,
            projectIdentifier,
            payload => !string.IsNullOrWhiteSpace(payload.SourceIssue?.SourceIssueUid),
            payload => MatchesOptionalId(issueIdentifier.IssueUid, payload.SourceIssue.SourceIssueUid));
    }

    private Task<WebhookResponse<TranslationIssueCreatedPayload>> HandleTranslationIssueCreatedCallback(
        WebhookRequest request,
        ProjectIdentifier projectIdentifier,
        IssueOptionalIdentifier issueIdentifier)
    {
        return HandleIssueCallback<TranslationIssueCreatedPayload>(
            request,
            TranslationIssueCreatedEventType,
            projectIdentifier,
            payload => !string.IsNullOrWhiteSpace(payload.TranslationIssue?.TranslationIssueUid),
            payload => MatchesOptionalId(issueIdentifier.IssueUid, payload.TranslationIssue.TranslationIssueUid));
    }

    private Task<WebhookResponse<SourceIssueCommentCreatedPayload>> HandleSourceIssueCommentCreatedCallback(
        WebhookRequest request,
        ProjectIdentifier projectIdentifier,
        IssueOptionalIdentifier issueIdentifier,
        IssueCommentOptionalIdentifier commentIdentifier)
    {
        return HandleIssueCallback<SourceIssueCommentCreatedPayload>(
            request,
            SourceIssueCommentCreatedEventType,
            projectIdentifier,
            payload => !string.IsNullOrWhiteSpace(payload.SourceIssue?.SourceIssueUid) &&
                       !string.IsNullOrWhiteSpace(payload.SourceIssueComment?.CommentUid),
            payload => MatchesOptionalId(issueIdentifier.IssueUid, payload.SourceIssue.SourceIssueUid) &&
                       MatchesOptionalId(commentIdentifier.CommentUid, payload.SourceIssueComment.CommentUid));
    }

    private Task<WebhookResponse<TranslationIssueCommentCreatedPayload>> HandleTranslationIssueCommentCreatedCallback(
        WebhookRequest request,
        ProjectIdentifier projectIdentifier,
        IssueOptionalIdentifier issueIdentifier,
        IssueCommentOptionalIdentifier commentIdentifier)
    {
        return HandleIssueCallback<TranslationIssueCommentCreatedPayload>(
            request,
            TranslationIssueCommentCreatedEventType,
            projectIdentifier,
            payload => !string.IsNullOrWhiteSpace(payload.TranslationIssue?.TranslationIssueUid) &&
                       !string.IsNullOrWhiteSpace(payload.TranslationIssueComment?.CommentUid),
            payload => MatchesOptionalId(issueIdentifier.IssueUid, payload.TranslationIssue.TranslationIssueUid) &&
                       MatchesOptionalId(commentIdentifier.CommentUid, payload.TranslationIssueComment.CommentUid));
    }

    private Task<WebhookResponse<T>> HandleIssueCallback<T>(
        WebhookRequest request,
        string expectedEventType,
        ProjectIdentifier projectIdentifier,
        Func<T, bool> containsIssueData,
        Func<T, bool> matchesIssueFilters) where T : class, IIssueWebhookPayload
    {
        var payload = TryDeserializePayload<T>(request);
        if (payload is null || !IsExpectedEventType(payload.EventType, expectedEventType) || !containsIssueData(payload))
        {
            InvocationContext.Logger?.LogWarning(
                $"[SmartlingIssueWebhook] Ignored a webhook request that is not a readable {expectedEventType} event. Body: {request.Body}",
                []);
            return Task.FromResult(GetPreflightResponse<T>());
        }

        var matchesFilters = MatchesProjectFilter(payload.Project?.ProjectUid, projectIdentifier) &&
                             matchesIssueFilters(payload);

        return Task.FromResult(matchesFilters ? GetEventResponse(payload) : GetPreflightResponse<T>());
    }

    private static bool MatchesOptionalId(string? filterId, string? payloadId)
    {
        return filterId == null || filterId == payloadId;
    }

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

    private Task<WebhookResponse<JobWorkflowStepReachedPayload>> HandleJobWorkflowStepReachedCallback(
        WebhookRequest request,
        ProjectIdentifier projectIdentifier,
        JobOptionalIdentifier jobIdentifier,
        TargetLocalesIdentifier targetLocalesIdentifier,
        WorkflowStepOptionalIdentifier workflowStepIdentifier)
    {
        var payload = TryDeserializePayload<JobWorkflowStepReachedPayload>(request);
        if (!IsJobWorkflowStepReachedEvent(payload))
        {
            InvocationContext.Logger?.LogWarning(
                $"[SmartlingJobWorkflowStepReached] Ignored a webhook request that is not a readable {JobWorkflowStepReachedEventType} event. Body: {request.Body}",
                []);
            return Task.FromResult(GetPreflightResponse<JobWorkflowStepReachedPayload>());
        }

        var matchesFilters = MatchesProjectFilter(payload, projectIdentifier) &&
                             MatchesJobWorkflowStepFilters(payload, jobIdentifier, targetLocalesIdentifier,
                                 workflowStepIdentifier);

        return Task.FromResult(matchesFilters
            ? GetEventResponse(payload)
            : GetPreflightResponse<JobWorkflowStepReachedPayload>());
    }

    private static WebhookResponse<T> GetEventResponse<T>(T payload) where T : class
    {
        return new WebhookResponse<T>
        {
            HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
            Result = payload
        };
    }

    private static bool IsExpectedEventType(string? eventType, string expectedEventType)
    {
        return string.IsNullOrWhiteSpace(eventType) ||
               string.Equals(eventType, expectedEventType, StringComparison.OrdinalIgnoreCase);
    }

    private static T? TryDeserializePayload<T>(WebhookRequest request) where T : class
    {
        var body = request.Body?.ToString();
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<T>(body,
                new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool IsJobWorkflowStepReachedEvent([NotNullWhen(true)] JobWorkflowStepReachedPayload? payload)
    {
        return payload is not null &&
               IsExpectedEventType(payload.EventType, JobWorkflowStepReachedEventType) &&
               !string.IsNullOrWhiteSpace(payload.TranslationJob?.JobUid) &&
               !string.IsNullOrWhiteSpace(payload.LocaleWorkflowStep?.WorkflowStep?.WorkflowStepUid);
    }

    private static bool MatchesJobWorkflowStepFilters(
        JobWorkflowStepReachedPayload payload,
        JobOptionalIdentifier jobIdentifier,
        TargetLocalesIdentifier targetLocalesIdentifier,
        WorkflowStepOptionalIdentifier workflowStepIdentifier)
    {
        if (!string.IsNullOrWhiteSpace(jobIdentifier.TranslationJobUid) &&
            !string.Equals(jobIdentifier.TranslationJobUid, payload.TranslationJob.JobUid,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var targetLocaleIds = targetLocalesIdentifier.TargetLocaleIds?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        if (targetLocaleIds?.Length > 0 &&
            !targetLocaleIds.Contains(payload.LocaleWorkflowStep.Locale?.LocaleId, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(workflowStepIdentifier.WorkflowStepUid) &&
            !string.Equals(workflowStepIdentifier.WorkflowStepUid,
                payload.LocaleWorkflowStep.WorkflowStep.WorkflowStepUid,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private static bool MatchesProjectFilter(
        TranslationJobWebhookPayload? payload,
        ProjectIdentifier projectIdentifier)
    {
        return MatchesProjectFilter(payload?.Project?.ProjectUid, projectIdentifier);
    }

    private static bool MatchesProjectFilter(string? payloadProjectUid, ProjectIdentifier projectIdentifier)
    {
        return string.IsNullOrWhiteSpace(projectIdentifier.ProjectId) ||
               string.Equals(projectIdentifier.ProjectId, payloadProjectUid, StringComparison.OrdinalIgnoreCase);
    }

    private WebhookResponse<FilePublishedPayload> HandleFilePublishedCallback(WebhookRequest request)
    {
        var jsonPayload = ParseJsonPayload(request.Body?.ToString());
        var queryParameters = ExtractQueryParameters(request);

        return new WebhookResponse<FilePublishedPayload>
        {
            HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
            Result = new FilePublishedPayload
            {
                Locale = GetJsonValue(jsonPayload, "file.publishedLocale.localeId")
                         ?? GetJsonValue(jsonPayload, "publishedLocale.localeId")
                         ?? GetJsonValue(jsonPayload, "locale")
                         ?? GetQueryValue(queryParameters, "locale"),
                PublishStatus = GetJsonValue(jsonPayload, "publishStatus")
                                ?? GetPublishStatusFromEventType(jsonPayload)
                                ?? GetQueryValue(queryParameters, "publishStatus"),
                FileUri = GetJsonValue(jsonPayload, "file.fileUri")
                          ?? GetJsonValue(jsonPayload, "fileUri")
                          ?? GetQueryValue(queryParameters, "fileUri"),
                Timestamp = GetJsonValue(jsonPayload, "file.publishDate")
                            ?? GetJsonValue(jsonPayload, "publishDate")
                            ?? GetQueryValue(queryParameters, "ts"),
                JobName = GetJsonValue(jsonPayload, "job.jobName")
                          ?? GetJsonValue(jsonPayload, "translationJob.jobName")
                          ?? GetJsonValue(jsonPayload, "translationJob.name")
                          ?? GetJsonValue(jsonPayload, "jobName")
            }
        };
    }

    private static bool ContainsFormattingIndifferent(string? value, string? searchValue)
    {
        var normalizedValue = NormalizeForContains(value);
        var normalizedSearchValue = NormalizeForContains(searchValue);

        if (string.IsNullOrEmpty(normalizedValue) || string.IsNullOrEmpty(normalizedSearchValue))
            return false;

        return normalizedValue.Contains(normalizedSearchValue, StringComparison.Ordinal);
    }

    private static string NormalizeForContains(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (char.IsLetterOrDigit(character))
                builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }

    private static Dictionary<string, string> ExtractQueryParameters(WebhookRequest request)
    {
        var queryParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        MergeValueIntoQueryParameters(request.Body?.ToString(), queryParameters);
        MergeValueIntoQueryParameters(TryGetRequestPropertyValue(request, "Url"), queryParameters);
        MergeValueIntoQueryParameters(TryGetRequestPropertyValue(request, "RequestUri"), queryParameters);
        MergeValueIntoQueryParameters(TryGetRequestPropertyValue(request, "Query"), queryParameters);

        return queryParameters;
    }

    private static JObject? ParseJsonPayload(string? rawBody)
    {
        if (string.IsNullOrWhiteSpace(rawBody))
            return null;

        var trimmedBody = rawBody.Trim();
        if (!trimmedBody.StartsWith("{"))
            return null;

        try
        {
            return JObject.Parse(trimmedBody);
        }
        catch
        {
            return null;
        }
    }

    private static string? GetJsonValue(JObject? payload, string jsonPath)
        => payload?.SelectToken(jsonPath)?.Value<string>();

    private static string? GetPublishStatusFromEventType(JObject? payload)
    {
        var eventType = payload?.SelectToken("eventType")?.Value<string>();
        return string.Equals(eventType, "file.published", StringComparison.OrdinalIgnoreCase)
            ? "published"
            : null;
    }

    private static string? TryGetRequestPropertyValue(WebhookRequest request, string propertyName)
    {
        var property = request.GetType().GetProperty(propertyName);
        if (property is null)
            return null;

        var value = property.GetValue(request);
        return value switch
        {
            null => null,
            Uri uri => uri.ToString(),
            string stringValue => stringValue,
            IDictionary dictionary => string.Join("&", dictionary.Keys
                .Cast<object>()
                .Select(key => $"{key}={dictionary[key]}")),
            _ => value.ToString()
        };
    }

    private static void MergeValueIntoQueryParameters(string? rawValue, IDictionary<string, string> queryParameters)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return;

        if (Uri.TryCreate(rawValue, UriKind.Absolute, out var absoluteUri))
        {
            MergeQueryString(absoluteUri.Query, queryParameters);
            return;
        }

        var queryCandidate = rawValue.StartsWith('?') ? rawValue : rawValue.Contains('=') ? rawValue : string.Empty;
        if (!string.IsNullOrEmpty(queryCandidate))
            MergeQueryString(queryCandidate, queryParameters);
    }

    private static void MergeQueryString(string queryString, IDictionary<string, string> queryParameters)
    {
        if (string.IsNullOrWhiteSpace(queryString))
            return;

        var trimmedQuery = queryString.TrimStart('?');
        foreach (var pair in trimmedQuery.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var pairParts = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(pairParts[0]);
            if (string.IsNullOrWhiteSpace(key))
                continue;

            var value = pairParts.Length > 1 ? Uri.UnescapeDataString(pairParts[1]) : string.Empty;
            queryParameters[key] = value;
        }
    }

    private static string? GetQueryValue(IReadOnlyDictionary<string, string> queryParameters, string key)
        => queryParameters.TryGetValue(key, out var value) ? value : null;
    
    private async Task<bool> IsFileFullyPublished(ProjectIdentifier projectIdentifier, string? fileUri, string? locale)
    {
        if (string.IsNullOrWhiteSpace(fileUri) || string.IsNullOrWhiteSpace(locale))
            return false;

        string projectId = await GetProjectId(projectIdentifier.ProjectId);
        var request = new SmartlingRequest($"/files-api/v2/projects/{projectId}/locales/{locale}/file/status", Method.Get)
            .AddQueryParameter("fileUri", fileUri);
        
        try
        {
            var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<FileLocaleStatusDto>>(request);
            var status = response.Response.Data;
            return status.CompletedStringCount + status.ExcludedStringCount >= status.TotalStringCount;
        }
        catch (Exception ex)
        {
            InvocationContext.Logger?.LogWarning($"Couldn't verify publish status for '{fileUri}' ({locale}): {ex.Message}", []);
            return false;
        }
    }
}
