using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Identifiers;

public class WorkflowStepIdentifier
{
    [Display("Workflow step ID")] 
    public string WorkflowStepUid { get; set; } = string.Empty;
}

public class WorkflowStepOptionalIdentifier
{
    [Display("Workflow step ID")]
    public string? WorkflowStepUid { get; set; }
}
