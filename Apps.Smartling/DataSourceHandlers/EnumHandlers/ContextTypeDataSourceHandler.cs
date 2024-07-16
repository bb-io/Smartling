using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers
{
    public class ContextTypeDataSourceHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData()
        {
            return new()
            {
                { "IMAGE", "Image" },
                { "HTML", "Html" },
                { "VIDEO", "Video" }
            };
        }
    }
}
