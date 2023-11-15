using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Jobs;

public class JobCancelledPayload : BaseJobsPayload
{
    [Display("Cancellation reason")]
    public string CancelReason { get; set; }
}