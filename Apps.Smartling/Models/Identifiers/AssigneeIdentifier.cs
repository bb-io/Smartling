using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class AssigneeIdentifier
{
    [Display("Assignee ID")]
    [DataSource(typeof(AssigneeDataSourceHandler))]
    public string? AssigneeUserUid { get; set; }
}