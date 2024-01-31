using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class AddGlossaryEntryTranslationRequest
{
    [Display("Term translation")]
    public string Term { get; set; }

    [Display("Term translation locale ID")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public string TermLocaleId { get; set; }

    [Display("Term translation is case sensitive")]
    public bool? TermCaseSensitive { get; set; }
    
    [Display("Exact term translation matching")]
    public bool? TermExactMatch { get; set; }
    
    [Display("Do not translate")]
    public bool? DoNotTranslate { get; set; }

    [Display("Term translation notes")]
    public string? TermNotes { get; set; }
    
    [Display("Translation disabled")]
    public bool? Disabled { get; set; }
    
    [Display("Term variations")]
    public IEnumerable<string>? TermVariations { get; set; }
}