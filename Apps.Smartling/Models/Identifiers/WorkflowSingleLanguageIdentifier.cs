using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class WorkflowSingleLanguageIdentifier
{
    [Display("Workflow ID")]
    [DataSource(typeof(WorkflowSingleLanguageHandler))]
    public string? WorkflowUid { get; set; }
}