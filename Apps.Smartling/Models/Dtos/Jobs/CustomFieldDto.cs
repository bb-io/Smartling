using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Jobs;

public class CustomFieldDto
{
    [Display("Field unique identifier")]
    public string FieldUid { get; set; }
    
    [Display("Field value")]
    public string FieldValue { get; set; }
}