using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Identifiers;

public class IssueCommentOptionalIdentifier
{
    [Display("Comment ID")]
    public string? CommentUid { get; set; }
}
