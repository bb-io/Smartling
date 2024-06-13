namespace Apps.Smartling.Models.Dtos;

public class WordCountDto
{
    public string AccountName { get; set; } = string.Empty;
    
    public string ProjectName { get; set; } = string.Empty;
    
    public string TargetLocale { get; set; } = string.Empty;
    
    public string JobName { get; set; } = string.Empty;
    
    public string TranslationResourceName { get; set; } = string.Empty;
    
    public string WorkflowStepType { get; set; } = string.Empty;
    
    public string FuzzyProfileName { get; set; } = string.Empty;
    
    public string FuzzyTier { get; set; } = string.Empty;
    
    public double WordCount { get; set; }
    
    public double CharacterCount { get; set; }
    
    public double WeightedWordCount { get; set; }
}