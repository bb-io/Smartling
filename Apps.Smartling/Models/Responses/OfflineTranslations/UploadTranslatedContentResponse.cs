using Apps.Smartling.Models.Responses.OfflineTranslations.Api;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.OfflineTranslations;

public class UploadTranslatedContentResponse(ImportTranslationContentApiResponse response)
{
    [Display("Word count")]
    public int WordCount { get; set; } = response.WordCount;

    [Display("String count")]
    public int StringCount { get; set; } = response.StringCount;
}