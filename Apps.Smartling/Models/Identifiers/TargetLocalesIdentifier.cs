using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class TargetLocalesIdentifier
{
    [Display("Target locales")]
    [DataSource(typeof(TargetLocaleDataSourceHandler))]
    public IEnumerable<string>? TargetLocaleIds { get; set; }
}