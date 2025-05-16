using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Files
{
    public class fileTypeRequest
    {
        [Display("File type")]
        [StaticDataSource(typeof(FileTypeDataSourceHandler))]
        public string? Type {get; set;}
    }
}
