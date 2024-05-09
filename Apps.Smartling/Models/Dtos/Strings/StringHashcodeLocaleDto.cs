using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Smartling.Models.Dtos.Strings
{
    public class StringHashcodeLocaleDto
    {
        public string Hashcode { get; set; }

        [Display("Target locale ID")]
        public string TargetLocaleId { get; set; }
    }
}
