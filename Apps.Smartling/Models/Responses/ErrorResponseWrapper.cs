using Apps.Smartling.Models.Dtos;

namespace Apps.Smartling.Models.Responses;

public record ErrorResponseWrapper(ErrorResponseData Response);

public record ErrorResponseData(string Code, IEnumerable<ErrorDto> Errors);