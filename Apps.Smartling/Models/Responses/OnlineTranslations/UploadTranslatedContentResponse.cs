using Apps.Smartling.Models.Responses.OnlineTranslations.Api;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.OnlineTranslations;

public class UploadTranslatedContentResponse(ImportTranslationContentApiResponse response)
{
    [Display("Word count")]
    public int WordCount { get; set; } = response.WordCount;

    [Display("String count")]
    public int StringCount { get; set; } = response.StringCount;
}