﻿using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class SearchGlossaryEntriesRequest
{
    public string? Query { get; set; }
    
    [Display("Locales")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public IEnumerable<string>? LocaleIds { get; set; }
    
    [Display("Entry state")]
    [DataSource(typeof(EntryStateDataSourceHandler))]
    public string? EntryState { get; set; }
    
    [Display("Missing translation locale")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public string? MissingTranslationLocaleId { get; set; }
    
    [Display("Present translation locale")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public string? PresentTranslationLocaleId { get; set; }
    
    [Display("Do not translate (DNT) locale")]
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