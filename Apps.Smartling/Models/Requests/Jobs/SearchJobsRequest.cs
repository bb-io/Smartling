using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Jobs;

public class SearchJobsRequest
{
    [Display("Search account-wide")]
    public bool? SearchAccountWide { get; set; }

    [Display("Project IDs for account-wide search"), DataSource(typeof(ProjectDataSourceHandler))]
    public IEnumerable<string>? AccountWideSearchProjectIds { get; set; }

    [Display("Job name contains")]
    public string? JobNameContains { get; set; }

    [Display("Created before")]
    public DateTime? CreatedDateBefore { get; set; }
    
    [Display("Created after")]
    public DateTime? CreatedDateAfter { get; set; }
    
    [Display("Due date before")]
    public DateTime? DueDateBefore { get; set; }
    
    [Display("Due date after")]
    public DateTime? DueDateAfter { get; set; }
    
    [Display("Job status")]
    [StaticDataSource(typeof(JobStatusDataSourceHandler))]
    public IEnumerable<string>? TranslationJobStatus { get; set; }
    
    [Display("Limit")]
    public int? Limit { get; set; }
}