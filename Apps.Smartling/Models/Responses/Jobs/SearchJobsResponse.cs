using Apps.Smartling.Models.Dtos.Jobs;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Jobs;
public class SearchJobsResponse
    {
        public IEnumerable<JobDto> Jobs { get; set; }

        [Display("Total count")]
        public double TotalCount { get; set; }
    }
