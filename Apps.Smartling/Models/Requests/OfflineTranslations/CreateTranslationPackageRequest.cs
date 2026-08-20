using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Requests.OfflineTranslations;

public class CreateTranslationPackageRequest
{
    [Display("Generate translation memory", Description = "Include a TMX (translation memory) resource. Default is false")]
    public bool? GenerateTmx { get; set; }
}