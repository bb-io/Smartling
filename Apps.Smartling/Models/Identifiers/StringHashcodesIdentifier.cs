using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Identifiers
{
    public class StringHashcodesIdentifier
    {
        [Display("Hashcodes")]
        public IEnumerable<string> Hashcodes { get; set; }
    }
}
