using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Jobs;

public class UpdateJobRequest
{
    [Display("Job name")]
    public string? JobName { get; set; }
    
    public string? Description { get; set; }

    [Display("Due date")]
    public DateTime? DueDate { get; set; }
    
    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }
    
    [Display("Callback URL")]
    public string? CallbackUrl { get; set; }
    
    [Display("Callback method")]
    [DataSource(typeof(CallbackMethodDataSourceHandler))]
    public string? CallbackMethod { get; set; }
}