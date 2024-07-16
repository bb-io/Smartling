using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class EntryStateDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "ACTIVE", "Active" },
            { "ARCHIVED", "Archived" },
            { "BOTH", "Active and archived" }
        };
    }
}