using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class IssueIdentifier
{
    [Display("Issue")]
    [DataSource(typeof(IssueDataSourceHandler))]
    public string IssueUid { get; set; }
}