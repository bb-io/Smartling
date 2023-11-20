using Apps.Smartling.Models.Dtos.Strings;

namespace Apps.Smartling.Models.Responses.Strings;

public record ListSourceStringsResponse(IEnumerable<SourceStringDto> Strings);