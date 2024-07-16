using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Smartling.Models.Requests.Context;

public class AddProjectContextRequest
{
    public string? Name { get; set; }

    [Display("Context file")]
    public FileReference ContextFile { get; set; }

    [Display("Run automatic context matching")]
    public bool? ContextMatching { get; set; }
}
