using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Issues;

public class SourceIssueCommentCreatedPayload
{
    [Display("Event ID")]
    public string EventId { get; set; }

    [Display("Event type")]
    public string EventType { get; set; }

    [Display("Schema version")]
    public string SchemaVersion { get; set; }

    public WebhookAccountDto Account { get; set; }

    public WebhookProjectDto Project { get; set; }

    [Display("Source issue")]
    public SourceIssueWebhookDto SourceIssue { get; set; }

    [Display("Source issue comment")]
    public WebhookIssueCommentDto SourceIssueComment { get; set; }
}
