namespace Apps.Smartling.Callbacks.Models.Payload.Issues;

public interface IIssueWebhookPayload
{
    string EventType { get; }

    WebhookProjectDto Project { get; }
}
