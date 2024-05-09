using Apps.Smartling.Models.Dtos.Strings;

namespace Apps.Smartling.Models.Responses.Strings
{
    public class ListStringsInJobResponse
    {
        public IEnumerable<StringHashcodeLocaleDto> Translations { get; set; }
    }
}
