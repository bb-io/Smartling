using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Context
{
    public class GetProjectContextRequest
    {
        [Display("Context ID")]
        [DataSource(typeof(ProjectContextDataSourceHandler))]
        public string ContextUid { get; set; }
    }
}
