using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Requests;

public class DatesOptionalRequest
{
    [Display("Start date", Description = "If not provided, the start date will be set to 2024-01-01")]
    public DateTime? StartDate { get; set; }
    
    [Display("End date", Description = "If not provided, the end date will be set to current date")]
    public DateTime? EndDate { get; set; }
}