using Apps.Smartling.Models.Dtos.Glossaries;

namespace Apps.Smartling.Polling.Models
{
    public class NewGlossaryEntriesResponse
    {
        public IEnumerable<GlossaryEntryDto> Entries { get; set; }

    }
}
