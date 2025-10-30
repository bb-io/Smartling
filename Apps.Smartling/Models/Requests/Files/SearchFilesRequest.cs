using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.Models.Requests.Files
{
    public class SearchFilesRequest
    {
        [Display("Uploaded after")]
        public DateTime? UploadedAfter { get; set; }

        [Display("Uploaded before")]
        public DateTime? UploadedBefore { get; set; }

        [Display("File type")]
        [StaticDataSource(typeof(FileTypeDataSourceHandler))]
        public IEnumerable<string>? FileTypes { get; set; }

        [Display("File URI contains")]
        public string? FileUriContains { get; set; }
    }
}
