using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Glossaries;

public class FallbackLocaleDto
{
    [Display("Fallback locale")]
    public string FallbackLocaleId { get; set; }
    
    [Display("Locales")]
    public IEnumerable<string> LocaleIds { get; set; }
}