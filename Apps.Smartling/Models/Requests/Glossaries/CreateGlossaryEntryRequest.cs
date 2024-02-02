using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class CreateGlossaryEntryRequest
{
    public string Term { get; set; }

    [Display("Term locale ID")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public string TermLocaleId { get; set; }
    
    public string Definition { get; set; }
    
    [Display("Term is case sensitive")]
    public bool? TermCaseSensitive { get; set; }
    
    [Display("Exact term matching")]
    public bool? TermExactMatch { get; set; }
    
    [Display("Do not translate term")]
    public bool? DoNotTranslate { get; set; }
    
    [Display("Part of speech")]
    [DataSource(typeof(PartOfSpeechDataSourceHandler))]
    public string? PartOfSpeech { get; set; }
    
    [Display("Labels")]
    [DataSource(typeof(GlossaryLabelDataSourceHandler))]
    public IEnumerable<string>? LabelUids { get; set; }

    [Display("Term notes")]
    public string? TermNotes { get; set; }
    
    [Display("Translation disabled")]
    public bool? Disabled { get; set; }
    
    [Display("Term variations")]
    public IEnumerable<string>? TermVariations { get; set; }
}