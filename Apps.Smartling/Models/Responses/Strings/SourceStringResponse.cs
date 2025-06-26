using Apps.Smartling.Models.Dtos.Strings;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.Strings;

public class SourceStringResponse
{
    public string Hashcode { get; set; }

    [Display("Parsed string text")]
    public string ParsedStringText { get; set; }

    [Display("String instructions")]
    public IEnumerable<string> StringInstructions { get; set; }

    [Display("String variant")]
    public string? StringVariant { get; set; }

    [Display("Maximum character length")]
    public int? MaxLength { get; set; }

    [Display("Content file string instruction")]
    public string? ContentFileInstructions { get; set; }

    public SourceStringResponse(SourceStringDto dto)
    {
        Hashcode = dto.Hashcode;
        ParsedStringText = dto.ParsedStringText;
        StringInstructions = dto.StringInstructions;
        StringVariant = dto.StringVariant;
        MaxLength = dto.MaxLength;
        ContentFileInstructions = dto.contentFileInstructions != null
            ? string.Join(" ",
                dto.contentFileInstructions
                   .Where(i => !string.IsNullOrWhiteSpace(i.Instruction))
                   .Select(i => $"{i.Instruction} ({i.FileUri})"))
            : "";
    }

    public SourceStringResponse() { }

}