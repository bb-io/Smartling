namespace Apps.Smartling.Models.Dtos.Issues;

public record IssueTypeDto(string IssueTypeCode, string Description, IEnumerable<IssueSubtypeDto> SubTypes);

public record IssueSubtypeDto(string IssueSubTypeCode, string Description, bool Enabled);