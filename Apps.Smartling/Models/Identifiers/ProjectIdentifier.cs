using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class ProjectIdentifier
{
    /// <summary>
    /// Can be provided from the connection input, therefore nullable.
    /// Use <see cref="SmartlingInvocable.GetProjectId"/> to resolve the project ID.
    /// </summary>
    [Display("Project ID"), DataSource(typeof(ProjectDataSourceHandler))]
    public string? ProjectId { get; set; }
}