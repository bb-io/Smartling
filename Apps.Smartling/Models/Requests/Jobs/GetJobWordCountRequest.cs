using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.Models.Requests.Jobs;

public class GetJobWordCountRequest
{
    [Display("Start date", Description = "If not provided, the start date will be set to 2024-01-01")]
    public DateTime? StartDate { get; set; }

    [Display("End date", Description = "If not provided, the end date will be set to current date")]
    public DateTime? EndDate { get; set; }

    [Display("Workflow step types")]
    [StaticDataSource(typeof(WorkflowStepTypeDataSourceHandler))]
    public IEnumerable<string>? WorkflowStepTypes { get; set; }
}
