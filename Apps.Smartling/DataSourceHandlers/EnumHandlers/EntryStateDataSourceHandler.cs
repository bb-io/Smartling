using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class EntryStateDataSourceHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "ACTIVE", "Active" },
        { "ARCHIVED", "Archived" },
        { "BOTH", "Active and archived" }
    };
}