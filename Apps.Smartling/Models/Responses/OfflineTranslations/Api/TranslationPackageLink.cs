using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.OfflineTranslations.Api;

public class TranslationPackageLink
{
    [JsonProperty("rel")]
    public string Rel { get; set; } = string.Empty;

    [JsonProperty("href")]
    public string Href { get; set; } = string.Empty;

    [JsonProperty("translationMemory")]
    public string? TranslationMemory { get; set; }
    
    [JsonProperty("glossary")]
    public string? Glossary { get; set; }
}