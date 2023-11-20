using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Strings;

public class StringTranslationsDto : StringDto
{
    [Display("Target locale")]
    public string TargetLocaleId { get; set; }
    
    [Display("Workflow step")]
    public string WorkflowStepUid { get; set; }
    
    public IEnumerable<StringTranslationDto> Translations { get; set; } 
}

public class StringTranslationDto
{
    public string Translation { get; set; }

    [Display("Date of last modification")]
    public DateTime ModifiedDate { get; set; }
    
    [Display("Plural form")]
    public string? PluralForm { get; set; }
}