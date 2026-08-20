using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.OfflineTranslations.Api;

public class TranslationPackageApiResponse
{
    [JsonProperty("translationPackageUid")]
    public string TranslationPackageUid { get; set; } = string.Empty;

    [JsonProperty("links")]
    public List<TranslationPackageLink> Links { get; set; } = [];
}