using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class JobStatusDataSourceHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
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