using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Identifiers;

public class TranslationPackageIdentifier
{
    [Display("Translation package ID")]
    public string TranslationPackageUid { get; set; } = string.Empty;
}