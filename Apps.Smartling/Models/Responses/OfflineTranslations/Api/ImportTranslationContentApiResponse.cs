using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.OfflineTranslations.Api;

public class ImportTranslationContentApiResponse
{
    [JsonProperty("wordCount")]
    public int WordCount { get; set; }
    
    [JsonProperty("stringCount")]
    public int StringCount { get; set; }

    [JsonProperty("translationImportErrors")]
    public List<TranslationImportError>? ImportErrors { get; set; }
}