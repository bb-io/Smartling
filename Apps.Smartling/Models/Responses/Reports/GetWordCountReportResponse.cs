using Apps.Smartling.Models.Dtos.Reports;

namespace Apps.Smartling.Models.Responses.Reports;

public record GetWordCountReportResponse(IEnumerable<WordCountReportDto> Reports);