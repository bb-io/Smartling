using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Jobs;

public class ScheduleItemDto
{
    [Display("Schedule UID")]
    public string ScheduleUid { get; set; }
    
    [Display("Target locale")]
    public string TargetLocaleId { get; set; }
    
    [Display("Workflow step UID")]
    public string WorkflowStepUid { get; set; }
    
    [Display("Due date")]
    public DateTime DueDate { get; set; }
}