using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Glossaries;

public class FallbackLocaleDto
{
    [Display("Fallback locale ID")]
    public string FallbackLocaleId { get; set; }
    
    [Display("Locale IDs")]
    public IEnumerable<string> LocaleIds { get; set; }
}