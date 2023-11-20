using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Strings;

public class SourceStringDto : StringDto
{
    [Display("String instructions")]
    public IEnumerable<string> StringInstructions { get; set; }
    
    [Display("String variant")]
    public string? StringVariant { get; set; }
    
    [Display("Maximum character length")]
    public int? MaxLength { get; set; }
}