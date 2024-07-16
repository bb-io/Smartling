using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class JobStatusDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "DRAFT", "Draft" },
            { "AWAITING_AUTHORIZATION", "Awaiting authorization" },
            { "IN_PROGRESS", "In progress" },
            { "COMPLETED", "Completed" },
            { "CANCELLED", "Cancelled" },
            { "CLOSED", "Closed" },
            { "DELETED", "Deleted" }
        };
    }
}