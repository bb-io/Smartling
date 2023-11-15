using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class SourceFileIdentifier
{
    [Display("Source file")]
    [DataSource(typeof(FileDataSourceHandler))]
    public string FileUri { get; set; }
}