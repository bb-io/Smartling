using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Requests.Tags
{
    public class AddTagsRequest
    {
        [Display("Tags")]
        public IEnumerable<string> Tags { get; set; }
    }
}
