using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class AddFallbackLocaleToGlossaryRequest
{
    [Display("Fallback locale ID")]
    [DataSource(typeof(AccountAvailableLocaleDataSourceHandler))]
    public string FallbackLocaleId { get; set; }
    
    [Display("Locale IDs")]
    [DataSource(typeof(FallbackLocaleLocaleDataSourceHandler))]
    public IEnumerable<string> LocaleIds { get; set; }
}