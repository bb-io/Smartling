using Apps.Smartling.Models.Dtos.Issues;

namespace Apps.Smartling.Models.Responses.Issues;

public record SearchIssuesResponse(IEnumerable<IssueDto> Issues);