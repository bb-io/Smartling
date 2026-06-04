using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Jobs;

public class FindJobByHashcodeItemDto
{
    [Display("Job ID")]
    public string TranslationJobUid { get; set; } = string.Empty;
}
