using System.Net;
using Apps.Smartling.Callbacks.Handlers;
using Apps.Smartling.Callbacks.Models.Payload.Jobs;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Smartling.Callbacks;

[WebhookList]
public class CallbackList
{
    #region Bridge callbacks

    [Webhook("On job completed", typeof(JobCompletedCallbackHandler), 
        Description = "This event is triggered when a job is completed.")]
    public Task<WebhookResponse<JobCompletedPayload>> OnJobCompleted(WebhookRequest request) 
        => HandleCallback<JobCompletedPayload>(request);
    
    [Webhook("On job cancelled", typeof(JobCancelledCallbackHandler), 
        Description = "This event is triggered when a job is cancelled.")]
    public Task<WebhookResponse<JobCancelledPayload>> OnJobCancelled(WebhookRequest request) 
        => HandleCallback<JobCancelledPayload>(request);
    
    #endregion

    #region Manual callbacks

    [Webhook("On job completed (manual)", Description = "This manual event is triggered when a job is completed.")]
    public Task<WebhookResponse<JobCompletedPayload>> OnJobCompletedManual(WebhookRequest request) 
        => HandleCallback<JobCompletedPayload>(request);
    
    [Webhook("On job cancelled (manual)", Description = "This manual event is triggered when a job is cancelled.")]
    public Task<WebhookResponse<JobCancelledPayload>> OnJobCancelledManual(WebhookRequest request) 
        => HandleCallback<JobCancelledPayload>(request);

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
}