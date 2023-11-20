using Apps.Smartling.Models.Dtos.Strings;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Strings;

public class StringPublishedPayload
{
    public string Hashcode { get; set; }
    
    [Display("Locale")]
    public string LocaleId { get; set; }
    
    public IEnumerable<StringTranslationDto> Translations { get; set; }
}