using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class SearchGlossaryEntriesRequest
{
    public string? Query { get; set; }
    
    [Display("Locale IDs")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public IEnumerable<string>? LocaleIds { get; set; }
    
    [Display("Entry state")]
    [StaticDataSource(typeof(EntryStateDataSourceHandler))]
    public string? EntryState { get; set; }
    
    [Display("Missing translation locale ID")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public string? MissingTranslationLocaleId { get; set; }
    
    [Display("Present translation locale ID")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public string? PresentTranslationLocaleId { get; set; }
    
    [Display("Do not translate (DNT) locale ID")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public string? DntLocaleId { get; set; }
    
    [Display("Return fallback translations")]
    public bool? ReturnFallbackTranslations { get; set; }
    
    [Display("Labels")]
    [DataSource(typeof(GlossaryLabelDataSourceHandler))]
    public IEnumerable<string>? LabelIds { get; set; }
    
    [Display("Do not translate (DNT) term is set")]
    public bool? DntTermSet { get; set; }
    
    [Display("Entry created before")]
    public DateTime? GlossaryEntryCreatedBefore { get; set; }
    
    [Display("Entry created after")]
    public DateTime? GlossaryEntryCreatedAfter { get; set; }
}