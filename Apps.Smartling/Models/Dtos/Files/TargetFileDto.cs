namespace Apps.Smartling.Models.Dtos.Files;

public record TargetFileDtoWrapper(IEnumerable<TargetFileDto> TargetFiles);

public record TargetFileDto(string TargetFileUri, string TargetFileType);