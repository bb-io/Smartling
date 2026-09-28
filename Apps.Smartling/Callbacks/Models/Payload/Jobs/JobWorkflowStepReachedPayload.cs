using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Jobs;

public class JobWorkflowStepReachedPayload : TranslationJobWebhookPayload
{
    [Display("Locale workflow step")]
    public LocaleWorkflowStepDto LocaleWorkflowStep { get; set; } = new();
}

public class LocaleWorkflowStepDto
{
    public TranslationJobWebhookLocaleDto Locale { get; set; } = new();

    [Display("Reached date")]
    public DateTime? ReachedDate { get; set; }

    public JobWorkflowDto Workflow { get; set; } = new();

    [Display("Workflow step")]
    public JobWorkflowStepDto WorkflowStep { get; set; } = new();
}

public class JobWorkflowDto
{
    [Display("Workflow ID")]
    public string WorkflowUid { get; set; } = string.Empty;

    [Display("Workflow name")]
    public string? WorkflowName { get; set; }

    [Display("Workflow steps")]
    public IEnumerable<JobWorkflowStepDto>? WorkflowSteps { get; set; }
}

public class JobWorkflowStepDto
{
    [Display("Workflow step ID")]
    public string WorkflowStepUid { get; set; } = string.Empty;

    [Display("Workflow step type")]
    public string? WorkflowStepType { get; set; }

    [Display("Workflow step name")]
    public string? WorkflowStepName { get; set; }
}
