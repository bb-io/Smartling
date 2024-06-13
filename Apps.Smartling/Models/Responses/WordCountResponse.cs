using Apps.Smartling.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses;

public class WordCountResponse
{
    [Display("Total word count")]
    public double TotalWordCount { get; set; }

    [Display("Total character count")]
    public double TotalCharacterCount { get; set; }
    
    [Display("Total weighted word count")]
    public double TotalWeightedWordCount { get; set; }

    [Display("Fuzzy tier (0 - 84.9%)")]
    public WordFuzzyTierCountResponse FuzzyTier84 { get; set; }
    
    [Display("Fuzzy tier (85 - 94.9%)")]
    public WordFuzzyTierCountResponse FuzzyTier94 { get; set; }
    
    [Display("Fuzzy tier (95 - 99.9%)")]
    public WordFuzzyTierCountResponse FuzzyTier99 { get; set; }
    
    [Display("Fuzzy tier (100%)")]
    public WordFuzzyTierCountResponse FuzzyTier100 { get; set; }
    
    [Display("Fuzzy tier (Repetition)")]
    public WordFuzzyTierCountResponse FuzzyTierRepetition { get; set; }

    public static WordCountResponse CreateFromDtos(IEnumerable<WordCountDto> dtos)
    {
        var response = new WordCountResponse();
        foreach (var dto in dtos)
        {
            switch (dto.FuzzyTier)
            {
                case "0 - 84.9%":
                    response.FuzzyTier84 = new WordFuzzyTierCountResponse
                    {
                        FuzzyTier = dto.FuzzyTier,
                        WordCount = dto.WordCount,
                        CharacterCount = dto.CharacterCount,
                        WeightedWordCount = dto.WeightedWordCount
                    };
                    break;
                case "85 - 94.9%":
                    response.FuzzyTier94 = new WordFuzzyTierCountResponse
                    {
                        FuzzyTier = dto.FuzzyTier,
                        WordCount = dto.WordCount,
                        CharacterCount = dto.CharacterCount,
                        WeightedWordCount = dto.WeightedWordCount
                    };
                    break;
                case "95 - 99.9%":
                    response.FuzzyTier99 = new WordFuzzyTierCountResponse
                    {
                        FuzzyTier = dto.FuzzyTier,
                        WordCount = dto.WordCount,
                        CharacterCount = dto.CharacterCount,
                        WeightedWordCount = dto.WeightedWordCount
                    };
                    break;
                case "100%":
                    response.FuzzyTier100 = new WordFuzzyTierCountResponse
                    {
                        FuzzyTier = dto.FuzzyTier,
                        WordCount = dto.WordCount,
                        CharacterCount = dto.CharacterCount,
                        WeightedWordCount = dto.WeightedWordCount
                    };
                    break;
                case "Repetition":
                    response.FuzzyTierRepetition = new WordFuzzyTierCountResponse
                    {
                        FuzzyTier = dto.FuzzyTier,
                        WordCount = dto.WordCount,
                        CharacterCount = dto.CharacterCount,
                        WeightedWordCount = dto.WeightedWordCount
                    };
                    break;
            }
        }
        
        response.TotalWordCount = dtos.Sum(x => x.WordCount);
        response.TotalCharacterCount = dtos.Sum(x => x.CharacterCount);
        response.TotalWeightedWordCount = dtos.Sum(x => x.WeightedWordCount);

        return response;
    }
}

public class WordFuzzyTierCountResponse
{
    [Display("Fuzzy tier")]
    public string FuzzyTier { get; set; }
    
    [Display("Word count")]
    public double WordCount { get; set; }
    
    [Display("Character count")]
    public double CharacterCount { get; set; }
    
    [Display("Weighted word count")]
    public double WeightedWordCount { get; set; }
}