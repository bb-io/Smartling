using Apps.Smartling.Models.Dtos.Locales;

namespace Apps.Smartling.Models.Dtos.Jobs;

public record WorkflowDto(string WorkflowUid, string WorkflowName, IEnumerable<SourceTargetLocalesPairDto> LocalePairs);
