using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Files
{
    public class fileUploadRequest
    {
        [Display("File type")]
        [StaticDataSource(typeof(FileTypeDataSourceHandler))]
        public string? Type {get; set;}

        public IEnumerable<string>? Directives { get; set; }

        [Display("Directive values")]
        public IEnumerable<string>? DirectiveValues { get; set; }
    }
}
