using Apps.Smartling.Models.Dtos.Tags;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Tags;

public class ListStringTagsResponse
{
    [Display("Items")]
    public IEnumerable<StringTagsResponseItem> Items { get; set; } = Array.Empty<StringTagsResponseItem>();

    [Display("Total count")]
    public int? TotalCount { get; set; }

    public ListStringTagsResponse(IEnumerable<StringTagsDto> items, int? totalCount)
    {
        Items = items.Select(x => new StringTagsResponseItem(x)).ToList();
        TotalCount = totalCount;
    }
}

public class StringTagsResponseItem
{
    [Display("String hashcode")]
    public string StringHashcode { get; set; } = string.Empty;

    [Display("Tags")]
    public IEnumerable<string> Tags { get; set; } = Array.Empty<string>();

    public StringTagsResponseItem(StringTagsDto dto)
    {
        StringHashcode = dto.StringHashcode;
        Tags = dto.Tags.Select(x => x.Tag).ToList();
    }
}
