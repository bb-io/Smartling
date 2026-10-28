using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.MachineTranslation;

public class ApplyMachineTranslationRequest
{
    [Display("Source strings", Description = "Strings to translate, paired with keys by array index. Provide 1 to 1000 strings.")]
    public IEnumerable<string> SourceStrings { get; set; } = [];

    [Display("Keys", Description = "Unique, non-empty keys, one for each source string in the same order.")]
    public IEnumerable<string> Keys { get; set; } = [];

    [Display("Source locale ID")]
    [DataSource(typeof(SmartlingLocaleDataSourceHandler))]
    public string SourceLocaleId { get; set; } = string.Empty;

    [Display("Target locale ID")]
    [DataSource(typeof(SmartlingLocaleDataSourceHandler))]
    public string TargetLocaleId { get; set; } = string.Empty;

    [Display("Profile ID", Description = "Optional MT or LLM profile ID. Leave empty to use the default profile configured in Smartling.")]
    public string? ProfileUid { get; set; }
}
