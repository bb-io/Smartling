using Apps.Smartling.Models.Dtos.Files;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Jobs;

public class JobDto
{
    [Display("Job")]
    public string TranslationJobUid { get; set; }
    
    [Display("Job name")]
    public string JobName { get; set; }
    
    [Display("Job status")]
    public string JobStatus { get; set; }
    
    public string? Description { get; set; }
    
    [Display("Target locales")]
    public IEnumerable<string>? TargetLocaleIds { get; set; }
    
    [Display("Due date")]
    public DateTime? DueDate { get; set; }
    
    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }
    
    [Display("Source files")]
    public IEnumerable<SourceFileDto>? SourceFiles { get; set; }

    [Display("Callback URL")]
    public string? CallbackUrl { get; set; }
    
    [Display("Callback method")]
    public string? CallbackMethod { get; set; }
    
    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }
    
    [Display("Date of last modification")]
    public DateTime ModifiedDate { get; set; }
    
    [Display("Created by user")]
    public string CreatedByUserUid { get; set; }
    
    [Display("Modified by user")]
    public string ModifiedByUserUid { get; set; }
    
    [Display("First completed date")]
    public DateTime? FirstCompletedDate { get; set; }
    
    [Display("Last completed date")]
    public DateTime? LastCompletedDate { get; set; }
    
    [Display("First authorized date")]
    public DateTime? FirstAuthorizedDate { get; set; }
    
    [Display("Last authorized date")]
    public DateTime? LastAuthorizedDate { get; set; }
    
    [Display("Custom fields")]
    public IEnumerable<CustomFieldDto>? CustomFields { get; set; }
}