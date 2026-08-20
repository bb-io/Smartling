using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Polling.Models.Requests;

public class OnTranslationPackageGeneratedRequest
{
    [Display("Also wait for glossary", 
        Description = "Only enable this if the package was created with 'Include glossary'. Default is false")]
    public bool? AlsoWaitForTbx { get; set; }

    [Display("Also wait for translation memory", 
        Description = "Only enable this if the package was created with 'Include translation memory'. Default is false")]
    public bool? AlsoWaitForTmx { get; set; }
}