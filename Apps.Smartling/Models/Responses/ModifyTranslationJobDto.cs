using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Apps.Smartling.Models.Responses
{
    public class ModifyTranslationJobDto
    {
        [JsonProperty("response")]
        public ResponseWrapper Response { get; set; }
    }
    public class ResponseWrapper
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("data")]
        public ResponseData Data { get; set; }
    }

    public class ResponseData
    {
        [JsonProperty("items")]
        public List<ScheduleItem> Items { get; set; }

        [JsonProperty("totalCount")]
        [DefinitionIgnore]
        public int TotalCount { get; set; }
    }

    public class ScheduleItem
    {
        [JsonProperty("scheduleUid")]
        [Display("Schedule UID")]
        public string ScheduleUid { get; set; }

        [JsonProperty("targetLocaleId")]
        [JsonPropertyName("targetLocaleId")]
        public string TargetLocaleId { get; set; }

        [JsonProperty("workflowStepUid")]
        [Display("Workflow step UID")]
        public string WorkflowStepUid { get; set; }

        [JsonProperty("dueDate")]
        [Display("Due date")]
        public DateTime DueDate { get; set; }
    }
}
