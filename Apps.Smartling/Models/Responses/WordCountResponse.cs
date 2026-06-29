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
    public WordFuzzyTierCountResponse FuzzyTier84 { get; set; } = CreateEmptyBucket("0 - 84.9%");
    
    [Display("Fuzzy tier (85 - 94.9%)")]
    public WordFuzzyTierCountResponse FuzzyTier94 { get; set; } = CreateEmptyBucket("85 - 94.9%");
    
    [Display("Fuzzy tier (95 - 99.9%)")]
    public WordFuzzyTierCountResponse FuzzyTier99 { get; set; } = CreateEmptyBucket("95 - 99.9%");
    
    [Display("Fuzzy tier (100%)")]
    public WordFuzzyTierCountResponse FuzzyTier100 { get; set; } = CreateEmptyBucket("100%");
    
    [Display("Fuzzy tier (Repetition)")]
    public WordFuzzyTierCountResponse FuzzyTierRepetition { get; set; } = CreateEmptyBucket("Repetition");

    public static WordCountResponse CreateFromDtos(IEnumerable<WordCountDto> dtos)
    {
        var response = new WordCountResponse();

        foreach (var groupedDtos in dtos.GroupBy(dto => NormalizeFuzzyTier(dto.FuzzyTier)))
        {
            var fuzzyTier = groupedDtos.Key;
            if (fuzzyTier is null)
                continue;

            var bucket = new WordFuzzyTierCountResponse
            {
                FuzzyTier = fuzzyTier,
                WordCount = groupedDtos.Sum(dto => dto.WordCount),
                CharacterCount = groupedDtos.Sum(dto => dto.CharacterCount),
                WeightedWordCount = groupedDtos.Sum(dto => dto.WeightedWordCount)
            };

            switch (fuzzyTier)
            {
                case "0 - 84.9%":
                    response.FuzzyTier84 = bucket;
                    break;
                case "85 - 94.9%":
                    response.FuzzyTier94 = bucket;
                    break;
                case "95 - 99.9%":
                    response.FuzzyTier99 = bucket;
                    break;
                case "100%":
                    response.FuzzyTier100 = bucket;
                    break;
                case "Repetition":
                    response.FuzzyTierRepetition = bucket;
                    break;
            }
        }
        
        response.TotalWordCount = dtos.Sum(x => x.WordCount);
        response.TotalCharacterCount = dtos.Sum(x => x.CharacterCount);
        response.TotalWeightedWordCount = dtos.Sum(x => x.WeightedWordCount);

        return response;
    }

    private static string? NormalizeFuzzyTier(string fuzzyTier) => fuzzyTier switch
    {
        "0 - 49.9%" or "50 - 74.9%" or "75 - 84.9%" or "0 - 84.9%" => "0 - 84.9%",
        "85 - 94.9%" => "85 - 94.9%",
        "95 - 99.9%" => "95 - 99.9%",
        "100%" => "100%",
        "Repetition" => "Repetition",
        _ => null
    };

    private static WordFuzzyTierCountResponse CreateEmptyBucket(string fuzzyTier) => new()
    {
        FuzzyTier = fuzzyTier,
        WordCount = 0,
        CharacterCount = 0,
        WeightedWordCount = 0
    };
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
