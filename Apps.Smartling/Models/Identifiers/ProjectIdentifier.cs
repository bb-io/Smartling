using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class ProjectIdentifier
{
    [Display("Project ID")]
    [DataSource(typeof(ProjectDataSourceHandler))]
    public string? ProjectId { get; set; }
}