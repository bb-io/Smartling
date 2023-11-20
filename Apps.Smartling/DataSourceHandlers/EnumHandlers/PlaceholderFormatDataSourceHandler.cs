using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class PlaceholderFormatDataSourceHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "none", "None" },
        { "c", "C" },
        { "ios", "iOS" },
        { "python", "Python" },
        { "java", "Java" },
        { "yaml", "YAML" },
        { "qt", "QT" },
        { "resx", "RESX" }
    };
}