using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class UpdateGlossaryEntryRequest
{
    public string? Definition { get; set; }
    
    [Display("Part of speech")]
    [DataSource(typeof(PartOfSpeechDataSourceHandler))]
    public string? PartOfSpeech { get; set; }
    
    [Display("Labels")]
    [DataSource(typeof(GlossaryLabelDataSourceHandler))]
    public IEnumerable<string>? LabelUids { get; set; }
}