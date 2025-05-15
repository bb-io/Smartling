using Apps.Smartling.Models.Dtos.Jobs;

namespace Apps.Smartling.Models.Responses.Jobs;
public class SearchJobsResponse
    {
        public IEnumerable<JobDto> Jobs { get; set; }
    }
