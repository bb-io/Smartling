using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.OnlineTranslations.Api;

public class ImportTranslationContentApiResponse
{
    [JsonProperty("wordCount")]
    public int WordCount { get; set; }
    
    [JsonProperty("stringCount")]
    public int StringCount { get; set; }

    [JsonProperty("translationImportErrors")]
    public List<TranslationImportError>? ImportErrors { get; set; }
}