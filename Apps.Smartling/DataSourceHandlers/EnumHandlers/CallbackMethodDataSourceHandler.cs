using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class CallbackMethodDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        { 
            { "GET", "GET" },
            { "POST", "POST" } 
        };
    }
}