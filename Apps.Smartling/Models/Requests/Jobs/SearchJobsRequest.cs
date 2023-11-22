using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Jobs;

public class SearchJobsRequest
{
    [Display("Created before")]
    public DateTime? CreatedDateBefore { get; set; }
    
    [Display("Created after")]
    public DateTime? CreatedDateAfter { get; set; }
    
    [Display("Due date before")]
    public DateTime? DueDateBefore { get; set; }
    
    [Display("Due date after")]
    public DateTime? DueDateAfter { get; set; }
    
    [Display("Job status")]
    [DataSource(typeof(JobStatusDataSourceHandler))]
    public IEnumerable<string>? TranslationJobStatus { get; set; }
    
    public int? Limit { get; set; }
}