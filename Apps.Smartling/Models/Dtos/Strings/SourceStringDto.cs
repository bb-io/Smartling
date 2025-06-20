using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Smartling.Models.Dtos.Strings;

public class SourceStringDto : StringDto
{
    [Display("String instructions")]
    public IEnumerable<string> StringInstructions { get; set; }
    
    [Display("String variant")]
    public string? StringVariant { get; set; }
    
    [Display("Maximum character length")]
    public int? MaxLength { get; set; }

    [Display("Content file string instructions")]
    [JsonProperty("contentfilestringinstructions")]
    public IEnumerable<contentFileInstruction> ? contentFileInstructions { get; set; }
}

public class contentFileInstruction
{
    [JsonProperty("fileUri")]
    [Display("File Uri")]
    public string FileUri { get; set; }

    [JsonProperty("contentFileStringInstruction")]
    [Display("Content file string instruction")]
    public string Instruction {  get; set; }
}