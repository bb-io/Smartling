using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Jobs;

public class JobWorkflowStepReachedPayload
{
    [Display("Event ID")]
    public string EventId { get; set; } = string.Empty;

    [Display("Event type")]
    public string EventType { get; set; } = string.Empty;

    [Display("Schema version")]
    public string SchemaVersion { get; set; } = string.Empty;

    public JobWorkflowStepAccountDto Account { get; set; } = new();

    public JobWorkflowStepProjectDto Project { get; set; } = new();

    [Display("Translation job")]
    public JobWorkflowStepJobDto TranslationJob { get; set; } = new();

    [Display("Locale workflow step")]
    public LocaleWorkflowStepDto LocaleWorkflowStep { get; set; } = new();
}

public class JobWorkflowStepAccountDto
{
    [Display("Account ID")]
    public string AccountUid { get; set; } = string.Empty;

    [Display("Account name")]
    public string? AccountName { get; set; }
}

public class JobWorkflowStepProjectDto
{
    [Display("Project ID")]
    public string ProjectUid { get; set; } = string.Empty;

    [Display("Project name")]
    public string? ProjectName { get; set; }

    [Display("Project type")]
    public string? ProjectTypeCode { get; set; }

    [Display("Source locale")]
    public JobWorkflowStepLocaleDto? SourceLocale { get; set; }

    [Display("Target locales")]
    public IEnumerable<JobWorkflowStepLocaleDto>? TargetLocales { get; set; }
}

public class JobWorkflowStepJobDto
{
    [Display("Job ID")]
    public string JobUid { get; set; } = string.Empty;

    [Display("Job name")]
    public string? JobName { get; set; }

    [Display("Job status")]
    public string? JobStatus { get; set; }

    [Display("Job number")]
    public string? JobNumber { get; set; }

    [Display("Job description")]
    public string? JobDescription { get; set; }

    [Display("Job due date")]
    public DateTime? JobDueDate { get; set; }

    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }

    [Display("Rush request")]
    public bool? RushRequest { get; set; }

    [Display("Created date")]
    public DateTime? CreatedDate { get; set; }

    [Display("Updated date")]
    public DateTime? UpdatedDate { get; set; }

    [Display("Target locales")]
    public IEnumerable<JobWorkflowStepLocaleDto>? TargetLocales { get; set; }

    public IEnumerable<JobWorkflowStepFileDto>? Files { get; set; }
}

public class JobWorkflowStepFileDto
{
    [Display("File ID")]
    public string? FileUid { get; set; }

    [Display("File URI")]
    public string? FileUri { get; set; }

    [Display("Locale IDs")]
    public IEnumerable<string>? LocaleIds { get; set; }
}

public class JobWorkflowStepLocaleDto
{
    [Display("Locale ID")]
    public string LocaleId { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class LocaleWorkflowStepDto
{
    public JobWorkflowStepLocaleDto Locale { get; set; } = new();

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
