using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.OnlineTranslations.Api;

public class TranslationPackageApiResponse
{
    [JsonProperty("translationPackageUid")]
    public string TranslationPackageUid { get; set; } = string.Empty;

    [JsonProperty("links")]
    public TranslationPackageLinks Links { get; set; } = null!;
}