using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Smartling.Models.Requests.Context;

public class AddProjectContextRequest
{
    public string? Name { get; set; }

    [Display("Context file")]
    public FileReference ContextFile { get; set; }

    [Display("Run automatic context matching")]
    public bool? ContextMatching { get; set; }

    [Display("Job ID")]
    [DataSource(typeof(JobDataSourceHandler))]
    public string? TranslationJobUid { get; set; }

    [Display("String hashcodes")]
    public IEnumerable<string>? StringHashcodes { get; set; }

    [Display("Content file URI")]
    public string? ContentFileUri { get; set; }

    [Display("Override context older than (days)")]
    public int? OverrideContextOlderThanDays { get; set; }
}
