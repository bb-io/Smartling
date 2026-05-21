using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class IssueOptionalIdentifier
{
    [Display("Issue ID")]
    [DataSource(typeof(IssueDataSourceHandler))]
    public string? IssueUid { get; set; }
}
