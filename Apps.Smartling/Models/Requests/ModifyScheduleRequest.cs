using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Apps.Smartling.Models.Requests
{
    public class ModifyScheduleRequest
    {
        [DataSource(typeof(WorkflowDataSourceHandler))]
        [Display("Workflow step UID")]
        [JsonProperty("workflowStepUid")]
        public string WorkflowUid { get; set; }

        [Display("Due date")]
        [JsonProperty("dueDate")]
        public DateTime DueDate { get; set; }

        [Display("Target locale ID")]
        [DataSource(typeof(TargetLocaleDataSourceHandler))]
        [JsonProperty("targetLocaleId")]
        public string TargetLocaleId { get; set; }
    }
}
