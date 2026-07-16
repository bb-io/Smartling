using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Tags;

public class StringTagsDto
{
    [Display("Tags")]
    public IEnumerable<TagDto> Tags { get; set; } = Array.Empty<TagDto>();

    [Display("String hashcode")]
    public string StringHashcode { get; set; } = string.Empty;
}

public class TagDto
{
    [Display("Tag")]
    public string Tag { get; set; } = string.Empty;
}
