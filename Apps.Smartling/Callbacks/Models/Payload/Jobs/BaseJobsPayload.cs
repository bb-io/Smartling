using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Jobs;

public class BaseJobsPayload
{
    [Display("Job ID")]
    public string TranslationJobUid { get; set; }
}