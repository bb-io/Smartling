using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Strings;

public class StringDto
{
    public string Hashcode { get; set; }
    
    [Display("Parsed string text")]
    public string ParsedStringText { get; set; }
}