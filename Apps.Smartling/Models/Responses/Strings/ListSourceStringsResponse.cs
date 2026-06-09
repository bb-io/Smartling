using Apps.Smartling.Models.Dtos.Strings;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Strings;

public class ListSourceStringsResponse
{
    public IEnumerable<SourceStringDto> Strings { get; set; }

    [Display("String hashcodes")]
    public IEnumerable<string> StringHashcodes { get; set; }

    public ListSourceStringsResponse(IEnumerable<SourceStringDto> strings)
    {
        Strings = strings;
        StringHashcodes = strings.Select(x => x.Hashcode).ToList();
    }
}