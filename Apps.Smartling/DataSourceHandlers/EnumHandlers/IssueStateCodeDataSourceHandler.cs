using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class IssueStateCodeDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "OPENED", "Opened" },
            { "RESOLVED", "Resolved" }
        };
    }
}