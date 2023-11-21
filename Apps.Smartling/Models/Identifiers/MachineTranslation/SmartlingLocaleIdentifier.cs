using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers.MachineTranslation;

public class SmartlingSourceLocaleIdentifier
{
    [Display("Source locale")]
    [DataSource(typeof(SmartlingLocaleDataSourceHandler))]
    public string? LocaleId { get; set; }
}

public class SmartlingTargetLocaleIdentifier
{
    [Display("Target locale")]
    [DataSource(typeof(SmartlingLocaleDataSourceHandler))]
    public string LocaleId { get; set; }
}

public class SmartlingTargetLocalesIdentifier
{
    [Display("Target locales")]
    [DataSource(typeof(SmartlingLocaleDataSourceHandler))]
    public IEnumerable<string> LocaleIds { get; set; }
}