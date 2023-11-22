using Apps.Smartling.Models.Dtos.Jobs;

namespace Apps.Smartling.Models.Responses.Jobs;

public record SearchJobsResponse(IEnumerable<JobDto> Jobs);