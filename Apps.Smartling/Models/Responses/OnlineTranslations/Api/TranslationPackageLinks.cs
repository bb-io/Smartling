using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.OnlineTranslations.Api;

public class TranslationPackageLinks
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("translationMemory")]
    public string? TranslationMemory { get; set; }
    
    [JsonProperty("glossary")]
    public string? Glossary { get; set; }
}