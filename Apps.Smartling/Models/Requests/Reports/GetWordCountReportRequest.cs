using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Reports;

public class GetWordCountReportRequest
{
    [Display("Start date (without time)")]
    public DateTime StartDate { get; set; }
    
    [Display("End date (without time)")]
    public DateTime? EndDate { get; set; }
    
    [Display("Job IDs")]
    [DataSource(typeof(JobDataSourceHandler))]
    public IEnumerable<string>? JobUids { get; set; }
    
    [Display("Target locale IDs")]
    [DataSource(typeof(TargetLocaleDataSourceHandler))]
    public IEnumerable<string>? TargetLocaleIds { get; set; }
}