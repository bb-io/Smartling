using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class PlaceholderFormatDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
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
}