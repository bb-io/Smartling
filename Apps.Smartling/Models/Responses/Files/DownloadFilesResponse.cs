namespace Apps.Smartling.Models.Responses.Files;

public record DownloadFilesResponse(IEnumerable<FileWrapper> Files);