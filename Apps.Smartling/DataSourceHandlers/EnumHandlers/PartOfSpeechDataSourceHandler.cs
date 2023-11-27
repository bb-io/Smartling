using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class PartOfSpeechDataSourceHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "Noun", "Noun" },
        { "Verb", "Verb" },
        { "Adjective", "Adjective" },
        { "Adverb", "Adverb" },
        { "Pronoun", "Pronoun" },
        { "Preposition", "Preposition" },
        { "Interjection", "Interjection" },
        { "Conjunction", "Conjunction" },
        { "Proper Noun", "Proper Noun" }
    };
}