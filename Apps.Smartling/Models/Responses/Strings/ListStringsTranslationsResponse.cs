using Apps.Smartling.Models.Dtos.Strings;

namespace Apps.Smartling.Models.Responses.Strings;

public record ListStringsTranslationsResponse(IEnumerable<StringTranslationsDto> Translations);