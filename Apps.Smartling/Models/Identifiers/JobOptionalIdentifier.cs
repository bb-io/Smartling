using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class JobOptionalIdentifier
{
    [Display("Job ID")]
    [DataSource(typeof(JobDataSourceHandler))]
    public string? TranslationJobUid { get; set; }
}