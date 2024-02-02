using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Reports;

public class WordCountReportDto
{
    [Display("Account name")]
    public string AccountName { get; set; }

    [Display("Account ID")]
    public string? AccountUid { get; set; }

    [Display("Agency name")]
    public string? AgencyName { get; set; }

    [Display("Agency ID")]
    public string? AgencyUid { get; set; }

    [Display("Fuzzy match profile name")]
    public string FuzzyProfileName { get; set; }

    [Display("Fuzzy match tier")]
    public string FuzzyTier { get; set; }

    [Display("Job name")]
    public string JobName { get; set; }

    [Display("Job ID")]
    public string JobUid { get; set; }

    [Display("Target locale ID")]
    public string? TargetLocaleId { get; set; }

    [Display("Target locale description")]
    public string TargetLocale { get; set; }

    [Display("Translation resource name")]
    public string TranslationResourceName { get; set; }

    [Display("Translation resource ID")]
    public string TranslationResourceUid { get; set; }

    [Display("Number of weighted words")]
    public int WeightedWordCount { get; set; }

    [Display("Number of words")]
    public int WordCount { get; set; }

    [Display("Number of characters")]
    public int CharacterCount { get; set; }

    [Display("Workflow step type")]
    public string? WorkflowStepType { get; set; }

    [Display("Workflow step ID")]
    public string? WorkflowStepUid { get; set; }

    [Display("Workflow step name")]
    public string? WorkflowStepName { get; set; }
}