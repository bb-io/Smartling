using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Glossaries;

public class GlossaryDto
{
    [Display("Glossary")]
    public string GlossaryUid { get; set; }
    
    [Display("Account unique identifier")]
    public string AccountUid { get; set; }
    
    [Display("Glossary name")]
    public string GlossaryName { get; set; }
    
    public string Description { get; set; }
    
    public bool Archived { get; set; }
    
    [Display("Verification mode")]
    public bool VerificationMode { get; set; }
    
    [Display("Created by user")]
    public string CreatedByUserUid { get; set; }
    
    [Display("Modified by user")]
    public string ModifiedByUserUid { get; set; }
    
    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }
    
    [Display("Date of last modification")]
    public DateTime ModifiedDate { get; set; }
    
    [Display("Locales")]
    public IEnumerable<string> LocaleIds { get; set; }
    
    [Display("Fallback locales")]
    public IEnumerable<FallbackLocaleDto> FallbackLocales { get; set; }
}