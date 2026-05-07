using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Smartling.Models.Dtos.Issues;

public class IssueCommentDto
{
    [Display("Comment text")]
    public string CommentText { get; set; }

    [Display("Created by user ID")]
    [JsonProperty("createdByUserUid")]
    public string? CreatedByUserUid { get; set; }

    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }

    [Display("Comment ID")]
    public string IssueCommentUid { get; set; }

    [JsonProperty("createdByUserId")]
    private string? CreatedByUserIdFallback
    {
        set => CreatedByUserUid ??= value;
    }
}
