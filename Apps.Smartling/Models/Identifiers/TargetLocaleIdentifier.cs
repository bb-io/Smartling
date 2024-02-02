using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class TargetLocaleIdentifier
{
    [Display("Target locale ID")]
    [DataSource(typeof(TargetLocaleDataSourceHandler))]
    public string TargetLocaleId { get; set; }
}

public class TargetLocaleOptionalIdentifier
{
    [Display("Target locale ID")]
    [DataSource(typeof(TargetLocaleDataSourceHandler))]
    public string? TargetLocaleId { get; set; }
}

