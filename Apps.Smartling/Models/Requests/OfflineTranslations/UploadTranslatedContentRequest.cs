using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Smartling.Models.Requests.OfflineTranslations;

public class UploadTranslatedContentRequest
{
    [Display("File")]
    public FileReference File { get; set; } = null!;

    [Display("Submit content", Description = "If true, imported content is submitted to the next workflow step. Default is false")]
    public bool? SubmitContent { get; set; }

    [Display("Workflow step ID", Description = "Forces all content in this step to be submitted to the next step")]
    public string? WorkflowStepUid { get; set; }
}