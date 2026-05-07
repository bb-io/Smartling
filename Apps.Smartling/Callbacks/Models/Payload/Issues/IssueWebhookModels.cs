using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Issues;

public class WebhookAccountDto
{
    [Display("Account ID")]
    public string AccountUid { get; set; }
}

public class WebhookProjectDto
{
    [Display("Project ID")]
    public string ProjectUid { get; set; }
}

public class WebhookUserReferenceDto
{
    [Display("User ID")]
    public string? UserUid { get; set; }
}

public class WebhookUserMentionsDto
{
    public IEnumerable<WebhookUserReferenceDto>? Users { get; set; }
}

public class WebhookIssueLevelDto
{
    public string Code { get; set; }

    public string? Description { get; set; }
}

public class WebhookLocaleReferenceDto
{
    [Display("Locale")]
    public string LocaleId { get; set; }
}

public class WebhookIssueCommentDto
{
    [Display("Comment ID")]
    public string CommentUid { get; set; }

    [Display("Created by user")]
    public WebhookUserReferenceDto? CreatedByUser { get; set; }

    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }

    [Display("Date of update")]
    public DateTime? UpdatedDate { get; set; }

    public WebhookUserMentionsDto? Mentions { get; set; }

    [Display("Comment text")]
    public string Text { get; set; }
}

public class SourceIssueWebhookDto
{
    public bool Answered { get; set; }

    [Display("Assignee user")]
    public WebhookUserReferenceDto? AssigneeUser { get; set; }

    public IEnumerable<WebhookIssueCommentDto>? Comments { get; set; }

    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }

    public string Hashcode { get; set; }

    public bool Reactivated { get; set; }

    [Display("Date of reopening")]
    public DateTime? ReopenedDate { get; set; }

    [Display("Reported by user")]
    public WebhookUserReferenceDto? ReportedByUser { get; set; }

    [Display("Resolved by user")]
    public WebhookUserReferenceDto? ResolvedByUser { get; set; }

    [Display("Date of resolution")]
    public DateTime? ResolvedDate { get; set; }

    public WebhookUserMentionsDto? SourceIssueMentions { get; set; }

    [Display("Issue number")]
    public int SourceIssueNumber { get; set; }

    [Display("Issue severity level")]
    public WebhookIssueLevelDto? SourceIssueSeverityLevel { get; set; }

    [Display("Issue state")]
    public WebhookIssueLevelDto? SourceIssueState { get; set; }

    [Display("Issue subtype")]
    public WebhookIssueLevelDto? SourceIssueSubType { get; set; }

    [Display("Issue text")]
    public string SourceIssueText { get; set; }

    [Display("Issue ID")]
    public string SourceIssueUid { get; set; }
}

public class TranslationIssueWebhookDto
{
    public bool Answered { get; set; }

    [Display("Assignee user")]
    public WebhookUserReferenceDto? AssigneeUser { get; set; }

    public IEnumerable<WebhookIssueCommentDto>? Comments { get; set; }

    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }

    public string Hashcode { get; set; }

    [Display("Locale")]
    public WebhookLocaleReferenceDto? Locale { get; set; }

    public bool Reactivated { get; set; }

    [Display("Date of reopening")]
    public DateTime? ReopenedDate { get; set; }

    [Display("Reported by user")]
    public WebhookUserReferenceDto? ReportedByUser { get; set; }

    [Display("Resolved by user")]
    public WebhookUserReferenceDto? ResolvedByUser { get; set; }

    [Display("Date of resolution")]
    public DateTime? ResolvedDate { get; set; }

    public WebhookUserMentionsDto? TranslationIssueMentions { get; set; }

    [Display("Issue number")]
    public int TranslationIssueNumber { get; set; }

    [Display("Issue severity level")]
    public WebhookIssueLevelDto? TranslationIssueSeverityLevel { get; set; }

    [Display("Issue state")]
    public WebhookIssueLevelDto? TranslationIssueState { get; set; }

    [Display("Issue subtype")]
    public WebhookIssueLevelDto? TranslationIssueSubType { get; set; }

    [Display("Issue text")]
    public string TranslationIssueText { get; set; }

    [Display("Issue ID")]
    public string TranslationIssueUid { get; set; }
}
