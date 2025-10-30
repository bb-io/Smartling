using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Requests.Files
{
    public class SourceFileSummaryDto
    {
        [Display("File URI")]
        public string FileUri { get; set; } = string.Empty;

        [Display("File type")]
        public string? FileType { get; set; }

        [Display("Last uploaded")]
        public DateTime? LastUploaded { get; set; }
    }
}
