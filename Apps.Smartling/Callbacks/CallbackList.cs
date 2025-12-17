using System.Net;
using Apps.Smartling.Callbacks.Handlers;
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
    public async Task<WebhookResponse<StringPublishedPayload>> OnStringPublished(WebhookRequest request,
        [WebhookParameter] StringOptionalIdentifier optionalIdentifier)
    {
        var result = await HandleCallback<StringPublishedPayload>(request);
        if (optionalIdentifier.Hashcode != null && optionalIdentifier.Hashcode == result.Result?.Hashcode)
        {
            return GetPreflightResponse<StringPublishedPayload>();
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
    public async Task<WebhookResponse<StringPublishedPayload>> OnStringPublishedManual(WebhookRequest request,
        [WebhookParameter] StringOptionalIdentifier optionalIdentifier)
    {
        var result = await HandleCallback<StringPublishedPayload>(request);
        if (optionalIdentifier.Hashcode != null && optionalIdentifier.Hashcode == result.Result?.Hashcode)
        {
            return GetPreflightResponse<StringPublishedPayload>();
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