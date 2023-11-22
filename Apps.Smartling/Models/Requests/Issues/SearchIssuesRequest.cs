using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Issues;

public class SearchIssuesRequest
{
    [Display("Created before")]
    public DateTime? CreatedDateBefore { get; set; }
    
    [Display("Created after")]
    public DateTime? CreatedDateAfter { get; set; }
    
    [Display("Resolved before")]
    public DateTime? ResolvedDateBefore { get; set; }
    
    [Display("Resolved after")]
    public DateTime? ResolvedDateAfter { get; set; }
    
    public bool? Answered { get; set; } 
    
    public bool? Reopened { get; set; } 
    
    [Display("Issue severity level codes")]
    [DataSource(typeof(IssueSeverityLevelCodeDataSourceHandler))]
    public IEnumerable<string>? IssueSeverityLevelCodes { get; set; }
    
    [Display("Issue state code")]
    [DataSource(typeof(IssueStateCodeDataSourceHandler))]
    public string? IssueStateCode { get; set; }
    
    [Display("Issue type codes")]
    [DataSource(typeof(IssueTypeCodeDataSourceHandler))]
    public string? IssueTypeCode { get; set; }

    [Display("Assignee")]
    [DataSource(typeof(AssigneeDataSourceHandler))]
    public string? AssigneeUserUid { get; set; }
    
    [Display("Has comments")]
    public bool? HasComments { get; set; }
    
    [Display("Jobs")]
    [DataSource(typeof(JobDataSourceHandler))]
    public IEnumerable<string>? JobIds { get; set; }
    
    [Display("String hashcodes")]
    public IEnumerable<string>? StringHashcodes { get; set; }
    
    public int? Limit { get; set; }
}