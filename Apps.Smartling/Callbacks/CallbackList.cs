using System.Net;
using System.Collections;
using System.Reflection;
using System.Text;
using Apps.Smartling.Actions;
using Apps.Smartling.Callbacks.Handlers;
using Apps.Smartling.Callbacks.Models.Payload.Files;
using Apps.Smartling.Callbacks.Models.Payload.Issues;
using Apps.Smartling.Callbacks.Models.Payload.Jobs;
using Apps.Smartling.Callbacks.Models.Payload.Strings;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Jobs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Smartling.Callbacks;

[WebhookList]
public class CallbackList(InvocationContext invocationContext)
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
            var matchesJob = await MatchesJobNameContainsAsync(projectIdentifier, result.Result?.FileUri, jobNameContains);
            if (!matchesJob)
                return GetPreflightResponse<FilePublishedPayload>();
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

    private WebhookResponse<FilePublishedPayload> HandleFilePublishedCallback(WebhookRequest request)
    {
        var queryParameters = ExtractQueryParameters(request);

        return new WebhookResponse<FilePublishedPayload>
        {
            HttpResponseMessage = new HttpResponseMessage(statusCode: HttpStatusCode.OK),
            Result = new FilePublishedPayload
            {
                Locale = GetQueryValue(queryParameters, "locale"),
                PublishStatus = GetQueryValue(queryParameters, "publishStatus"),
                FileUri = GetQueryValue(queryParameters, "fileUri"),
                Timestamp = GetQueryValue(queryParameters, "ts")
            }
        };
    }

    private async Task<bool> MatchesJobNameContainsAsync(
        ProjectIdentifier projectIdentifier,
        string? fileUri,
        string jobNameContains)
    {
        if (string.IsNullOrWhiteSpace(projectIdentifier.ProjectId) || string.IsNullOrWhiteSpace(fileUri))
            return false;

        var jobActions = new JobActions(invocationContext);
        var fileActions = new FileActions(invocationContext, null!);

        var jobsResponse = await jobActions.SearchJobs(projectIdentifier, new SearchJobsRequest());
        var matchingJobs = jobsResponse.Jobs
            .Where(x => !string.IsNullOrWhiteSpace(x.TranslationJobUid) &&
                        ContainsFormattingIndifferent(x.JobName, jobNameContains));

        foreach (var job in matchingJobs)
        {
            var jobFiles = await fileActions.ListFilesWithinJob(projectIdentifier,
                new JobIdentifier { TranslationJobUid = job.TranslationJobUid });

            if (jobFiles.Files.Any(x => string.Equals(x.Uri, fileUri, StringComparison.OrdinalIgnoreCase)))
                return true;
        }

        return false;
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

        foreach (var property in request.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var value = property.GetValue(request);
            if (value is null)
                continue;

            switch (value)
            {
                case Uri uri:
                    MergeValueIntoQueryParameters(uri.Query, queryParameters);
                    break;
                case string stringValue:
                    MergeValueIntoQueryParameters(stringValue, queryParameters);
                    break;
                case IDictionary dictionary:
                    foreach (DictionaryEntry item in dictionary)
                    {
                        if (item.Key is not null && item.Value is not null)
                            queryParameters[item.Key.ToString()!] = item.Value.ToString()!;
                    }
                    break;
            }
        }

        return queryParameters;
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
}
