using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class TargetLocaleIdentifier
{
    [Display("Target locale")]
    [DataSource(typeof(TargetLocaleDataSourceHandler))]
    public string TargetLocaleId { get; set; }
}