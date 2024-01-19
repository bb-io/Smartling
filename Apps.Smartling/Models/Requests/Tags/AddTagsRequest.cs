using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Smartling.Models.Requests.Tags
{
    public class AddTagsRequest
    {
        [Display("Tags")]
        public IEnumerable<string> Tags { get; set; }
    }
}
