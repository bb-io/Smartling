using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class ExportGlossaryRequest
{
    [Display("Title", Description = "The name of the exported glossary.")]
    public string? Title { get; set; }
    
    [Display("Source description", Description = "Information or metadata about the source or origin of the " +
                                                 "terminology data contained in the glossary.")]
    public string? SourceDescription { get; set; }
    
    [Display("Locale IDs")]
    [DataSource(typeof(GlossaryTermLocaleDataSourceHandler))]
    public IEnumerable<string>? LocaleIds { get; set; }
    
    [Display("Entries state", Description = "If set, the result will include only entries with selected state.")]
    [StaticDataSource(typeof(EntryStateDataSourceHandler))]
    public string? EntriesState { get; set; }
    
    [Display("Return fallback translations", Description = "If set to 'True', appropriate fallback locales will " +
                                                           "be used for all missing translations.")]
    public bool? ReturnFallbackTranslations { get; set; }
    
    [Display("Labels", Description = "If set, the result will include only entries with selected labels.")]
    [DataSource(typeof(GlossaryLabelDataSourceHandler))]
    public IEnumerable<string>? LabelIds { get; set; }
    
    [Display("Entries modified before", Description = "If set, the result will include only entries created before " +
                                                    "specified date.")]
    public DateTime? GlossaryEntriesModifiedBefore { get; set; }
    
    [Display("Entries modified after", Description = "If set, the result will include only entries created after " +
                                                  "specified date.")]
    public DateTime? GlossaryEntriesModifiedAfter { get; set; }

    [Display("Export file format")]
    [StaticDataSource(typeof(GlossaryFileFormatSourceHandler))]
    public string? fileFormat { get; set; } 
}