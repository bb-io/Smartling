using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.Jobs;

public class JobProgressResponse
{
    public List<ContentProgressReportDto> ContentProgressReport { get; set; }
    public ProgressDto Progress { get; set; }
    public List<SummaryReportDto> SummaryReport { get; set; }

    public static JobProgressResponse CreateFromDto(JobProgressDto dto)
    {
        return new JobProgressResponse
        {
            ContentProgressReport = dto.ContentProgressReport,
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
    [Display("Target locale ID")]
    public string TargetLocaleId { get; set; }

    [Display("Target locale description")]
    public string TargetLocaleDescription { get; set; }

    [Display("Unauthorized progress report")]
    public UnauthorizedProgressReportDto UnauthorizedProgressReport { get; set; }

    [Display("Workflow progress report list")]
    public List<WorkflowProgressReportDto> WorkflowProgressReportList { get; set; }
    public ProgressDto Progress { get; set; }

    [Display("Summary report")]
    public List<SummaryReportDto> SummaryReport { get; set; }
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
    [Display("Workflow step type")]
    public string WorkflowStepType { get; set; }

    [Display("String count")]
    public double StringCount { get; set; }

    [Display("Word count")]
    public double WordCount { get; set; }

    [Display("Percent complete")]
    public double PercentComplete { get; set; }
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
