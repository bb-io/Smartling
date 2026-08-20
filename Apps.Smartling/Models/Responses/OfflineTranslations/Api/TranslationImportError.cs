using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.OfflineTranslations.Api;

public class TranslationImportError
{
    [JsonProperty("stringHashCode")]
    public string HashCode { get; set; } = string.Empty;

    [JsonProperty("messages")]
    public List<string>? Messages { get; set; }
}