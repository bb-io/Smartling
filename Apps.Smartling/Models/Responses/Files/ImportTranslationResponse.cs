using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Files;

public class ImportTranslationResponse
{
    [Display("Strings imported")]
    public int StringCount { get; set; }
    
    [Display("Words imported")]
    public int WordCount { get; set; }
}