using Apps.Smartling.Models.Dtos.Strings;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Strings
{
    public class ListStringsInJobResponse
    {
        public IEnumerable<StringHashcodeLocaleDto> Translations { get; set; }

        [Display("Total count")]
        public int TotalCount { get; set; }
    }
}
