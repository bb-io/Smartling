using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.Models.Requests.Context
{
    public class SearchProjectContextRequest
    {
        public string? Name { get; set; }

        [StaticDataSource(typeof(ContextTypeDataSourceHandler))]
        public string? Type { get; set; }
    }
}
