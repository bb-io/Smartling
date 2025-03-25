using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Smartling.Models.Responses.Context
{
    public class UploadContextResponseDto
    {
        [JsonProperty("contextUid")]
        [Display("Context ID")]
        public string? ContextUid { get; set; }

        [JsonProperty("processUid")]
        [Display("Process ID")]
        public string? ProcessUid { get; set; }
    }
}
