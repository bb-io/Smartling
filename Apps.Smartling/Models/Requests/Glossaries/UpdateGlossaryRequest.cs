using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class UpdateGlossaryRequest
{
    [Display("Glossary name")]
    public string? GlossaryName { get; set; }
    
    public string? Description { get; set; }
    
    [Display("Locales")]
    [DataSource(typeof(GlossaryLocaleDataSourceHandler))]
    public IEnumerable<string>? LocaleIds { get; set; }
}