using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Files;

public class FilePublishedPayload
{
    [Display("Locale")]
    public string? Locale { get; set; }

    [Display("Job name")]
    public string? JobName { get; set; }

    [Display("Publish status")]
    public string? PublishStatus { get; set; }

    [Display("Source file URI")]
    public string? FileUri { get; set; }

    [Display("Timestamp")]
    public string? Timestamp { get; set; }
}
