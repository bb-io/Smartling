using Apps.Smartling.Models.Dtos.MachineTranslation;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.MachineTranslation;

public class ApplyMachineTranslationResponse
{
    [Display("Translated strings", Description = "HTML translations in input key order. Failed items have an empty string; inspect Items for errors.")]
    public IEnumerable<string> TranslatedStrings { get; set; } = [];

    [Display("Items", Description = "Translation results and individual errors in input key order.")]
    public IEnumerable<MachineTranslationItemDto> Items { get; set; } = [];
}
