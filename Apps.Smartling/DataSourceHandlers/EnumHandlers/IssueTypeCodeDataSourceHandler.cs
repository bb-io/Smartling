using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class IssueTypeCodeDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "SOURCE", "Source" },
            { "TRANSLATION", "Translation" }
        };
    }
}