using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Jobs
{
    public class ListStringsRequest
    {
        [Display("Locale")]
        [DataSource(typeof(TargetLocaleDataSourceHandler))]
        public string? targetLocaleId { get; set; }
    }
}
