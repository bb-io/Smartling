using Apps.Smartling.Actions;
using Apps.Smartling.Callbacks.Handlers.Base;
using Apps.Smartling.Callbacks.Models.Payload.Jobs;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Smartling.Callbacks.Handlers;

public class JobCancelledCallbackHandler(
    InvocationContext invocationContext,
    [WebhookParameter] JobOptionalIdentifier optionalIdentifier) : CallbackHandler(invocationContext),
    IAfterSubscriptionWebhookEventHandler<JobCancelledPayload>
{
    protected override string Event => "job.cancelled";

    public async Task<AfterSubscriptionEventResponse<JobCancelledPayload>> OnWebhookSubscribedAsync()
    {
        if (optionalIdentifier.TranslationJobUid == null)
        {
            return null!;
        }

        var jobActions = new JobActions(InvocationContext);
        var job = await jobActions.GetJob(new() { TranslationJobUid = optionalIdentifier.TranslationJobUid });

        if (job.JobStatus == "CANCELLED")
        {
            return new AfterSubscriptionEventResponse<JobCancelledPayload>
            {
                Result = new() { TranslationJobUid = optionalIdentifier.TranslationJobUid }
            };
        }

        return null!;
    }
}