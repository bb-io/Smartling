using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Smartling.Models.Identifiers
{
    public class StringHashcodesIdentifier
    {
        [Display("Hashcodes")]
        public IEnumerable<string> Hashcodes { get; set; }
    }
}
