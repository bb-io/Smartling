using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class CallbackMethodDataSourceHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "GET", "GET" },
        { "POST", "POST" }
    };
}