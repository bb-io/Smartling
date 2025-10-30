using Apps.Smartling.Models.Requests.Files;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Files
{
    public class SearchFilesResponse
    {
        [Display("Items")]
        public IEnumerable<SourceFileSummaryDto> Items { get; set; } = Array.Empty<SourceFileSummaryDto>();

        [Display("Total count")]
        public int TotalCount { get; set; }
    }
}
