using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Context
{
    public class SimpleLinkContextRequest
    {
        [Display("Context UID")]
        public string ContextUid { get; set; }

        [Display("String Hashcode")]
        public string StringHashcode { get; set; }
    }
}
