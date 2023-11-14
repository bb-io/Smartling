using Apps.Smartling.Models.Dtos.Files;

namespace Apps.Smartling.Models.Responses.Files;

public record ListFilesResponse(IEnumerable<SourceFileWithLocalesDto> Files);