using Apps.Smartling.Models.Responses.OfflineTranslations.Api;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.OfflineTranslations;

public class TranslationPackageDto(TranslationPackageApiResponse response)
{
    [Display("Translation package ID")]
    public string TranslationPackageUid { get; set; } = response.TranslationPackageUid;
}