using Apps.Smartling.Models.Dtos.Glossaries;

namespace Apps.Smartling.Models.Responses.Glossaries;

public record SearchGlossaryEntriesResponse(IEnumerable<GlossaryEntryDto> Entries);