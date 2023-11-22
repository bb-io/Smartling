using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Issues;

public class IssueDto
{
    [Display("Issue unique identifier")]
    public string IssueUid { get; set; }
    
    public bool Answered { get; set; }

    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }

    [Display("Issue severity level")]
    public string IssueSeverityLevelCode { get; set; }

    [Display("Issue state")]
    public string IssueStateCode { get; set; }

    [Display("Issue subtype")]
    public string IssueSubTypeCode { get; set; }

    [Display("Issue text")]
    public string IssueText { get; set; }

    [Display("Date of issue text last modification")]
    public DateTime? IssueTextModifiedDate { get; set; }

    [Display("Issue type")]
    public string IssueTypeCode { get; set; }

    [Display("Account unique identifier")]
    public string AccountUid { get; set; }

    [Display("Issue number")]
    public string IssueNumber { get; set; }

    [Display("Reported by user")]
    public string ReportedByUserUid { get; set; }

    [Display("Resolved by user")]
    public string? ResolvedByUserUid { get; set; }

    [Display("Assignee")]
    public string? AssigneeUserUid { get; set; }

    [Display("Date of resolution")]
    public DateTime? ResolvedDate { get; set; }
    
    public bool Reopened { get; set; }

    [Display("Reopened by user")]
    public string? ReopenedByUserUid { get; set; }

    [Display("Date of reopening")]
    public DateTime? ReopenedDate { get; set; }

    public IssueStringDto String { get; set; }
}

public class IssueStringDto
{
    public string Hashcode { get; set; }
    
    [Display("Locale")]
    public string? LocaleId { get; set; }
}