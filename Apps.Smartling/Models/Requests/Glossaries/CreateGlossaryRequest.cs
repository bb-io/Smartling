using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class CreateGlossaryRequest
{
    [Display("Glossary name")]
    public string GlossaryName { get; set; }
    
    public string? Description { get; set; }
    
    [Display("Locale IDs")]
    [DataSource(typeof(AccountAvailableLocaleDataSourceHandler))]
    public IEnumerable<string> LocaleIds { get; set; }
}