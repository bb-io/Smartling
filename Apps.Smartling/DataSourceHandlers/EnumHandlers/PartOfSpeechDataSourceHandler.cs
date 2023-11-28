using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class PartOfSpeechDataSourceHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "NOUN", "Noun" },
        { "VERB", "Verb" },
        { "ADJECTIVE", "Adjective" },
        { "ADVERB", "Adverb" },
        { "PRONOUN", "Pronoun" },
        { "PREPOSITION", "Preposition" },
        { "INTERJECTION", "Interjection" },
        { "CONJUNCTION", "Conjunction" },
        { "PROPER_NOUN", "Proper Noun" }
    };
}