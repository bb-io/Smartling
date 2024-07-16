using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class TranslationStateDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "PUBLISHED", "Published" },
            { "POST_TRANSLATION", "Post translation" }
        };
    }
}