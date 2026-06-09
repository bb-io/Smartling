using Apps.Smartling.Models.Dtos.Strings;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Strings;

public record ListSourceStringsResponse(
    IEnumerable<SourceStringDto> Strings,

    [Display("String hashcodes")]
    IEnumerable<string> StringHashcodes
);