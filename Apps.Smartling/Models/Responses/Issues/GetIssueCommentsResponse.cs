using Apps.Smartling.Models.Dtos.Issues;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Issues;

public record GetIssueCommentsResponse(
    [property: Display("Comments")] IEnumerable<IssueCommentDto> Comments,
    [property: Display("Total count")] int TotalCount);
