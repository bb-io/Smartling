using Apps.Smartling.Actions;
using Apps.Smartling.Callbacks.Handlers.Base;
using Apps.Smartling.Callbacks.Models.Payload.Jobs;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Smartling.Callbacks.Handlers;

public class JobCompletedCallbackHandler(
    InvocationContext invocationContext,
    [WebhookParameter] JobOptionalIdentifier optionalIdentifier,
    [WebhookParameter] ProjectIdentifier projectIdentifier) : CallbackHandler(invocationContext),
    IAfterSubscriptionWebhookEventHandler<JobCompletedPayload>
{
    protected override string Event => "job.completed";

    public async Task<AfterSubscriptionEventResponse<JobCompletedPayload>> OnWebhookSubscribedAsync()
    {
        await LogToWebhookAsync("OnWebhookSubscribedAsync invoked for job.completed callback.");


        if (optionalIdentifier.TranslationJobUid == null)
        {
            return null!;
        }
        
        var jobActions = new JobActions(InvocationContext);
        var job = await jobActions.GetJob(
            projectIdentifier,
            new() { TranslationJobUid = optionalIdentifier.TranslationJobUid }
        );

        await LogToWebhookAsync($"Retrieved job with status '{job.JobStatus}' for job UID: {optionalIdentifier.TranslationJobUid}");


        if (job.JobStatus == "COMPLETED")
        {
            return new AfterSubscriptionEventResponse<JobCompletedPayload>
            {
                Result = new() { TranslationJobUid = optionalIdentifier.TranslationJobUid }
            };
        }
        await LogToWebhookAsync($"Job {optionalIdentifier.TranslationJobUid} is not completed. No payload returned.");


        return null!;
    }
}