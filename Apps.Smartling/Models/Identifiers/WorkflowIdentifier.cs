using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class WorkflowIdentifier
{
    [Display("Workflow ID")]
    [DataSource(typeof(WorkflowDataSourceHandler))]
    public string? WorkflowUid { get; set; }
}