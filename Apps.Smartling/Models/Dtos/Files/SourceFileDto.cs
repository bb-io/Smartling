using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Files;

public class SourceFileDto
{
    public string Name { get; set; }
    
    [Display("File URI")]
    public string Uri { get; set; }
    
    [Display("File ID")]
    public string FileUid { get; set; }
}

public class SourceFileWithLocalesDto : SourceFileDto
{
    [Display("Locale IDs")]
    public IEnumerable<string> LocaleIds { get; set; }
}