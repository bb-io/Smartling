using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json.Linq;

namespace Apps.Smartling.Models.Dtos.MachineTranslation;

public class MachineTranslationItemDto
{
    public string Key { get; set; } = string.Empty;

    [Display("Translation text")]
    public string? TranslationText { get; set; }

    [Display("MT ID")]
    public string? MtUid { get; set; }

    public string? Provider { get; set; }

    [Display("Error", Description = "Item error details as JSON, if translation failed.")]
    public string? Error { get; set; }
}

internal class MachineTranslationApiItemDto
{
    public string Key { get; set; } = string.Empty;
    public string? TranslationText { get; set; }
    public string? MtUid { get; set; }
    public string? Provider { get; set; }
    public JToken? Error { get; set; }
}
