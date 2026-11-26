using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.Jobs;

public class JobProgressResponse
{
    [Display("Content progress report")]
    public List<ContentProgressReportOutput> ContentProgressReport { get; set; }

    public ProgressDto Progress { get; set; }

    [Display("Summary report")]
    public List<SummaryReportDto> SummaryReport { get; set; }

    public static JobProgressResponse CreateFromDto(JobProgressDto dto)
    {
        return new JobProgressResponse
        {
            ContentProgressReport = (dto.ContentProgressReport ?? [])
                .Select(ContentProgressReportOutput.CreateFromDto)
                .ToList(),
            Progress = dto.Progress,
            SummaryReport = dto.SummaryReport
        };
    }
}
public class JobProgressDto
{
    [JsonProperty("contentProgressReport")]
    [Display("Content progress report")]
    public List<ContentProgressReportDto> ContentProgressReport { get; set; }


    [JsonProperty("progress")]
    public ProgressDto Progress { get; set; }

    [Display("Summary report")]
    [JsonProperty("summaryReport")]
    public List<SummaryReportDto> SummaryReport { get; set; }
}

public class ContentProgressReportDto
{
    [JsonProperty("targetLocaleId")]
    public string TargetLocaleId { get; set; }

    [JsonProperty("targetLocaleDescription")]
    public string TargetLocaleDescription { get; set; }

    [JsonProperty("unauthorizedProgressReport")]
    public UnauthorizedProgressReportDto UnauthorizedProgressReport { get; set; }

    [JsonProperty("workflowProgressReportList")]
    public List<WorkflowProgressReportDto> WorkflowProgressReportList { get; set; }

    [JsonProperty("progress")]
    public ProgressDto Progress { get; set; }

    [JsonProperty("summaryReport")]
    public List<SummaryReportDto> SummaryReport { get; set; }
}

public class ContentProgressReportOutput
{
    [Display("Target locale ID")]
    public string TargetLocaleId { get; set; }

    [Display("Target locale description")]
    public string TargetLocaleDescription { get; set; }

    [Display("Unauthorized progress report")]
    public UnauthorizedProgressReportDto UnauthorizedProgressReport { get; set; }

    [Display("Workflow progress report list")]
    public List<WorkflowProgressReportOutput> WorkflowProgressReportList { get; set; }

    public ProgressDto Progress { get; set; }

    [Display("Summary report")]
    public List<SummaryReportDto> SummaryReport { get; set; }

    public static ContentProgressReportOutput CreateFromDto(ContentProgressReportDto dto)
    {
        return new ContentProgressReportOutput
        {
            TargetLocaleId = dto.TargetLocaleId,
            TargetLocaleDescription = dto.TargetLocaleDescription,
            UnauthorizedProgressReport = dto.UnauthorizedProgressReport,
            WorkflowProgressReportList = (dto.WorkflowProgressReportList ?? [])
                .Select(WorkflowProgressReportOutput.CreateFromDto)
                .ToList(),
            Progress = dto.Progress,
            SummaryReport = dto.SummaryReport
        };
    }
}

public class UnauthorizedProgressReportDto
{
    [Display("String count")]
    public double StringCount { get; set; }

    [Display("Word count")]
    public double WordCount { get; set; }
}

public class WorkflowProgressReportDto
{
    [JsonProperty("workflowName")]
    public string WorkflowName { get; set; }

    [JsonProperty("workflowUid")]
    public string WorkflowUid { get; set; }

    [JsonProperty("workflowStepSummaryReportItemList")]
    public List<WorkflowStepSummaryReportItemDto> WorkflowStepSummaryReportItemList { get; set; }
}

public class WorkflowProgressReportOutput
{
    [Display("Workflow name")]
    public string WorkflowName { get; set; }

    [Display("Workflow ID")]
    public string WorkflowUid { get; set; }

    [Display("Workflow step summary report")]
    public List<WorkflowStepSummaryReportItemOutput> WorkflowStepSummaryReport { get; set; }

    public static WorkflowProgressReportOutput CreateFromDto(WorkflowProgressReportDto dto)
    {
        return new WorkflowProgressReportOutput
        {
            WorkflowName = dto.WorkflowName,
            WorkflowUid = dto.WorkflowUid,
            WorkflowStepSummaryReport = (dto.WorkflowStepSummaryReportItemList ?? [])
                .Select(WorkflowStepSummaryReportItemOutput.CreateFromDto)
                .ToList()
        };
    }
}

public class WorkflowStepSummaryReportItemDto
{
    [JsonProperty("workflowStepUid")]
    public string WorkflowStepUid { get; set; }

    [JsonProperty("workflowStepName")]
    public string WorkflowStepName { get; set; }

    [JsonProperty("stringCount")]
    public double StringCount { get; set; }

    [JsonProperty("wordCount")]
    public double WordCount { get; set; }

    [JsonProperty("workflowStepType")]
    public string WorkflowStepType { get; set; }
}

public class WorkflowStepSummaryReportItemOutput
{
    [Display("Workflow step ID")]
    public string WorkflowStepUid { get; set; }

    [Display("Workflow step name")]
    public string WorkflowStepName { get; set; }

    [Display("String count")]
    public double StringCount { get; set; }

    [Display("Word count")]
    public double WordCount { get; set; }

    [Display("Workflow step type")]
    public string WorkflowStepType { get; set; }

    public static WorkflowStepSummaryReportItemOutput CreateFromDto(WorkflowStepSummaryReportItemDto dto)
    {
        return new WorkflowStepSummaryReportItemOutput
        {
            WorkflowStepUid = dto.WorkflowStepUid,
            WorkflowStepName = dto.WorkflowStepName,
            StringCount = dto.StringCount,
            WordCount = dto.WordCount,
            WorkflowStepType = dto.WorkflowStepType
        };
    }
}

public class ProgressDto
{
    [Display("Total word count")]
    public double TotalWordCount { get; set; }

    [Display("Percent complete")]
    public double PercentComplete { get; set; }
}

public class SummaryReportDto
{
    [Display("String count")]
    public double StringCount { get; set; }

    [Display("Word count")]
    public double WordCount { get; set; }

    [Display("Workflow step type")]
    public string WorkflowStepType { get; set; }
}
