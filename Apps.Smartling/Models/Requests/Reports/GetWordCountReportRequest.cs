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
    
    [Display("Jobs")]
    [DataSource(typeof(JobDataSourceHandler))]
    public IEnumerable<string>? JobUids { get; set; }
    
    [Display("Target locales")]
    [DataSource(typeof(TargetLocaleDataSourceHandler))]
    public IEnumerable<string>? TargetLocaleIds { get; set; }
}