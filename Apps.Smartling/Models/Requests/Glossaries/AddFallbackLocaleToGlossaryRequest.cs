using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class AddFallbackLocaleToGlossaryRequest
{
    [Display("Fallback locale")]
    [DataSource(typeof(GlossaryLocaleDataSourceHandler))]
    public string FallbackLocaleId { get; set; }
    
    [Display("Locales")]
    [DataSource(typeof(FallbackLocaleLocaleDataSourceHandler))]
    public IEnumerable<string> LocaleIds { get; set; }
}