using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class StringFormatDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "html", "HTML" },
            { "plain_text", "Plain text" },
            { "auto", "Auto" }
        };
    }
}