using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Jobs;

public class FindJobByHashcodeRequest
{
    [Display("Hashcode")]
    public string Hashcode { get; set; } = string.Empty;

    [Display("Language")]
    [DataSource(typeof(TargetLocaleDataSourceHandler))]
    public string? Language { get; set; }
}
